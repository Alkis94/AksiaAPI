namespace AksiaAPI.Models.Business
{
    public class Page
    {
        private const int DEFAULT_PAGE_INDEX = 1;
        private const int DEFAULT_PAGE_SIZE = 10;

        public Page()
        {
            PageIndex = DEFAULT_PAGE_INDEX;
            PageSize = DEFAULT_PAGE_SIZE;
        }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }
    }
}
