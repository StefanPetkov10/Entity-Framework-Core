namespace P02_FootballBetting.Data.Models.Enums
{
    //Enumerations are not entities in the DB
    //Enumerations are string representations of int value
    //In the DB -> int
    public enum Prediction
    {
        Draw = 0,
        Win = 1,
        Lose = 2
    }
}
