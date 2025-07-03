namespace GraduationProject.Data.Entities
{
    public class ProductPharmacy
    {
        public int ProductID { get; set; }
        public Product product { get; set; }

        public int PharmacyID { get; set; }
        public Pharmacy pharmacy { get; set; }
    }
}
