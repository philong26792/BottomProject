namespace Bottom_API.Helpers
{
    public class FilterMissingParam
    {
        public string MO_No {get;set;}
        public string Material_ID {get;set;}
        public string FromTime {get;set;}
        public string ToTime {get;set;}
        public string Supplier_ID {get;set;}
        public MissingNoOfBatch[] ListMissingNo {get;set;}

        public string Downloaded { get; set; }
        
        
    }

    public class MissingNoOfBatch {
        public string missingNo {get;set;}
        public string batch {get;set;}
    }
}