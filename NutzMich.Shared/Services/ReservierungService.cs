using NutzMich.Contracts.Interfaces;
using NutzMich.Shared.Interfaces;
using NutzMich.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutzMich.Shared.Services
{
    public class ReservierungService : ConnectionUsingServiceBase, IReservierungService
    {
        INotificationService _notificationService;
        IAngebotService _angebotService;

        public ReservierungService(INotificationService notificationService, IAngebotService angebotService) : base(Factory.GetIdentityService())
        {
            _notificationService = notificationService;
            _angebotService = angebotService;
        }

        public async Task<string> CheckReservierungAsync(Reservierung reservierung)
        {
            var reservierungen = await GetReservierungenAsync(reservierung.AnbieterID, reservierung.AngebotID);
            foreach(var res in reservierungen)
            {
                if((res.Von < reservierung.Zeitraum.Von && reservierung.Zeitraum.Von < res.Bis ) || //Von überschneidet sich
                   (res.Von < reservierung.Zeitraum.Bis && reservierung.Zeitraum.Bis < res.Bis ) || //Bis überschneidet sich
                   (reservierung.Zeitraum.Von < res.Von && res.Bis < reservierung.Zeitraum.Bis ) )  //Bestehende Reservierung würde "geschluckt"
                {
                    return "In diesem Zeitraum liegt bereits eine Reservierung.";
                }
            }
            return "";
        }

        public async Task<bool> SaveReservierungAsync(Reservierung reservierung)
        {
            await InitWriteConnectionAsync();

            try
            {
                var reservierungBytes = Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(reservierung));
                var upload = await _writeConnection.ObjectService.UploadObjectAsync(_writeConnection.Bucket, "Angebote/" + reservierung.AnbieterID + "/" + reservierung.AngebotID + "/Reservierungen/" + GetTimeStamp(reservierung.Zeitraum), new uplink.NET.Models.UploadOptions() { Expires = reservierung.Zeitraum.Bis.DateTime.AddDays(1) }, reservierungBytes, false);
                await upload.StartUploadAsync();

                return upload.Completed;
            }
            catch
            {
                return false;
            }
        }

        private string GetTimeStamp(ReservierungsZeitraum reservierungsZeitraum)
        {
            return reservierungsZeitraum.Von.Year + "-" + reservierungsZeitraum.Von.Month + "-" + reservierungsZeitraum.Von.Day + "-" +
                   reservierungsZeitraum.Bis.Year + "-" + reservierungsZeitraum.Bis.Month + "-" + reservierungsZeitraum.Bis.Day;
        }

        private ReservierungsZeitraum GetReservierungsZeitraumVonKey(string key)
        {
            var parts = key.Split('-');
            ReservierungsZeitraum zeitraum = new ReservierungsZeitraum();
            zeitraum.Von = new DateTimeOffset(Convert.ToInt32(parts[0]), Convert.ToInt32(parts[1]), Convert.ToInt32(parts[2]), 0, 0, 0, TimeSpan.Zero);
            zeitraum.Bis = new DateTimeOffset(Convert.ToInt32(parts[3]), Convert.ToInt32(parts[4]), Convert.ToInt32(parts[5]), 0, 0, 0, TimeSpan.Zero);

            return zeitraum;
        }

        public async Task<List<ReservierungsZeitraum>> GetReservierungenAsync(string anbieterID, string angebotID)
        {
            await InitReadConnectionAsync();

            List<ReservierungsZeitraum> zeitraeume = new List<ReservierungsZeitraum>();

            string prefix = "Angebote/" + anbieterID + "/" + angebotID + "/Reservierungen/";

            var list = await _readConnection.ObjectService.ListObjectsAsync(_readConnection.Bucket, new uplink.NET.Models.ListObjectsOptions() { Prefix = prefix });
            foreach (var entry in list.Items)
            {
                zeitraeume.Add(GetReservierungsZeitraumVonKey(entry.Key.Replace(prefix, "")));
            }

            return zeitraeume.OrderBy(z => z.Von).ToList();
        }

        public async Task<Reservierung> GetReservierungAsync(ReservierungsZeitraum zeitraum, string anbieterID, string angebotID)
        {
            await InitReadConnectionAsync();

            var download = await _readConnection.ObjectService.DownloadObjectAsync(_readConnection.Bucket, "Angebote/" + anbieterID + "/" + angebotID + "/Reservierungen/" + GetTimeStamp(zeitraum), new uplink.NET.Models.DownloadOptions(), false);
            await download.StartDownloadAsync();

            if(download.Completed)
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<Reservierung>(Encoding.UTF8.GetString(download.DownloadedBytes));
            }
            else
            {
                return null;
            }
        }

        public async Task ReservierungBestaetigenAsync(Reservierung reservierung)
        {
            var angebot = await _angebotService.LoadAngebotAsync(reservierung.AnbieterID+"/"+reservierung.AngebotID);
#if DEBUG
            _notificationService.SendScheduledReservierungNotification("Reservierungs-Erinnerung!", "'" + angebot.Ueberschrift + "' wurde für dich morgen reserviert - denke an die Abholung!", DateTimeOffset.Now.AddSeconds(10));
#endif
            _notificationService.SendScheduledReservierungNotification("Reservierungs-Erinnerung!", "'" + angebot.Ueberschrift + "' wurde für dich morgen reserviert - denke an die Abholung!", reservierung.Zeitraum.Von.AddDays(-1).Date);
        }
    }
}
