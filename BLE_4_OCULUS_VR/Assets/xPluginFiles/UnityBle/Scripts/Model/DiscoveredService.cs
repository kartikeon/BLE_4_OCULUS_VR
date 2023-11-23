using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlePlugin.Data {
    public class DiscoveredService {
        private Dictionary<string, List<DiscoveredCharacterisitc>> discoveredServices = new Dictionary<string, List<DiscoveredCharacterisitc>>();

        // Set data when generating Dictionary
        public void setValue(string service, DiscoveredCharacterisitc characteristic) 
        {
            if (discoveredServices.ContainsKey(service)) {
                // If key already exists
                List<DiscoveredCharacterisitc> charas = discoveredServices[service];
                charas.Add(characteristic);
            } else {
                // If not yet, I will
                discoveredServices.Add(
                    service,
                    new List<DiscoveredCharacterisitc>{characteristic}
                );
            }
        }

        // getter
        public Dictionary<string, List<DiscoveredCharacterisitc>> GetDiscoveredServices() 
        {
            return new Dictionary<string, List<DiscoveredCharacterisitc>>(discoveredServices);
        }
        public List<string> GetServices() 
        {
            return new List<string>(discoveredServices.Keys);
        }
        public List<DiscoveredCharacterisitc> GetCharacteristics(string service)
        {
            return new List<DiscoveredCharacterisitc>(discoveredServices[service]);
        }
    }
}