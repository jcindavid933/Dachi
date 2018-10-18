using System;

namespace dojodachi
{
    public class dojodachi
    {
        public int fullness {get;set;}
        public int happiness {get;set;}
        public int meals {get;set;}
        public int energy {get;set;}
        public string status {get;set;}
        public dojodachi()
        {
            fullness = 20;
            happiness = 20;
            meals = 3;
            energy = 50;
            status = "Welcome to Dojodachi!";
        }
    }
}