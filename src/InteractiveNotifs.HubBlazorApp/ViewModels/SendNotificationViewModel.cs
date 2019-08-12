using AdaptiveBlocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InteractiveNotifs.HubBlazorApp.ViewModels
{
    public class SendNotificationViewModel
    {
        private string _payload;
        public string Payload
        {
            get => _payload;
            set
            {
                _payload = value;
                OnPayloadChanged();
            }
        }

        public bool IsPayloadInvalid { get; private set; }

        public AdaptiveBlock CurrentBlock { get; private set; }

        private void OnPayloadChanged()
        {
            try
            {
                var newBlock = AdaptiveBlock.Parse(Payload).Block;
                if (newBlock != null)
                {
                    CurrentBlock = newBlock;
                    IsPayloadInvalid = false;
                    return;
                }
            }
            catch
            {
            }

            IsPayloadInvalid = true;
        }

        public bool Sending { get; private set; }

        public void SendNotif()
        {
            Sending = true;
        }
    }
}
