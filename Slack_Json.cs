using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace RegisterParcelsFromPC
{

        //{"ok":true,"no_op":true,"already_open":true,"channel":{"id":"D02CGGQABPG"}}
        public class Channel
        {
            public string id { get; set; }
        }

        public class Root
        {
            public bool ok { get; set; }
            public bool no_op { get; set; }
            public bool already_open { get; set; }
            public Channel channel { get; set; }
        }

    
}
