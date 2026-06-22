namespace Core.Specifications
{
    public class CertificateSpecParams : PagingParams
    {
        public string? Sort { get; set; }
        public string? Status { get; set; }
        private string? _search;
        public string Search 
        { 
            get => _search ?? ""; 
            set => _search = value?.Trim().ToLower(); 
        }
    }
}