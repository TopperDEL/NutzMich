using NutzMich.Contracts.Interfaces;
using NutzMich.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using uplink.NET.Models;

namespace NutzMich.Shared.Services
{
    static class Factory
    {
        static IIdentityService _identityService;
        public static IIdentityService GetIdentityService()
        {
            if (_identityService == null)
                _identityService = new IdentityService(GetLoginService());

            return _identityService;
        }

        static IAngebotService _angebotService;
        public static IAngebotService GetAngebotService()
        {
            if (_angebotService == null)
                _angebotService = new AngebotService(GetIdentityService(), GetLoginService(), GetThumbnailHelper());

            return _angebotService;
        }

        static ILoginService _loginService;
        public static ILoginService GetLoginService()
        {
            if (_loginService == null)
                _loginService = new LoginService();

            return _loginService;
        }

        static IThumbnailHelper _thumbnailHelper;
        public static IThumbnailHelper GetThumbnailHelper()
        {
            if (_thumbnailHelper == null)
                _thumbnailHelper = new ThumbnailHelper();

            return _thumbnailHelper;
        }

        static IChatService _chatService;
        public static IChatService GetChatService()
        {
            if (_chatService == null)
                _chatService = new ChatService(GetIdentityService(), GetLoginService(), GetChatBufferService(), GetReservierungService());

            return _chatService;
        }

        static IChatPollingService _chatPollingService;
        public static IChatPollingService GetChatPollingService()
        {
            if (_chatPollingService == null)
                _chatPollingService = new ChatPollingService(GetChatService());

            return _chatPollingService;
        }

        static IChatBufferService _chatBufferService;
        public static IChatBufferService GetChatBufferService()
        {
            if (_chatBufferService == null)
                _chatBufferService = new ChatBufferService(GetLoginService());

            return _chatBufferService;
        }

        static IChatController _chatController;
        public static IChatController GetChatController()
        {
            if (_chatController == null)
                _chatController = new ChatController(GetAngebotService(), GetChatPollingService(), GetChatBufferService());

            return _chatController;
        }

        static INotificationService _notificationService;
        public static INotificationService GetNotificationService()
        {
            if (_notificationService == null)
                _notificationService = new NotificationService();

            return _notificationService;
        }

        static IReservierungService _reservierungService;
        public static IReservierungService GetReservierungService()
        {
            if (_reservierungService == null)
                _reservierungService = new ReservierungService(GetNotificationService(), GetAngebotService(), GetLoginService());

            return _reservierungService;
        }

        static IProfilService _profilService;
        public static IProfilService GetProfilService()
        {
            if (_profilService == null)
                _profilService = new ProfilService(GetIdentityService(), GetLoginService(), GetThumbnailHelper());

            return _profilService;
        }

        public static void Reset()
        {
            _identityService = null;
            _angebotService = null;
            _loginService = null;
        }
    }
}
