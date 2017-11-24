namespace AspergillosisEPR.Models
{
    public class PatientDrugSideEffect
    {
        public int ID { get; set; }
        public int PatientDrugId { get; set; }
        public int SideEffectId { get; set; }

        public PatientDrug PatientDrug { get; set; }
        public SideEffect SideEffect { get; set; }

    }
}
