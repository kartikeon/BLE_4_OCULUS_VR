using System.Collections.Generic;

namespace BlePlugin.Data {
    /*
    defined UUID: 0000XXXX-0000-1000-8000-00805f9b34fb
    */
    public class DiscoveredCharacterisitc {
        // Unique characteristic uuid
        public readonly string characteristicUuid;

        // characteristic service uuid
        public readonly string serviceUuid;

        // property
        public readonly bool isReadable;
        public readonly bool isWritableWithoutResponse;
        public readonly bool isWritableWithResponse;
        public readonly bool isNotifiable;
        public readonly bool isIndicatable;

        public DiscoveredCharacterisitc (
            string characteristicUuid,
            string serviceId,
            bool isReadable,
            bool isWritableWithoutResponse,
            bool isWritableWithResponse,
            bool isNotifiable,
            bool isIndicatable
        ) {
            this.characteristicUuid = characteristicUuid;
            this.serviceUuid = serviceId;
            this.isReadable = isReadable;
            this.isWritableWithoutResponse = isWritableWithoutResponse;
            this.isWritableWithResponse = isWritableWithResponse;
            this.isNotifiable = isNotifiable;
            this.isIndicatable = isIndicatable;
        }
    }

    public enum AttProperty {
        Read,

        WriteWithoutResponse,
        
        WriteWithResponse,
        
        Notify,
        
        Indicate,
    }
}