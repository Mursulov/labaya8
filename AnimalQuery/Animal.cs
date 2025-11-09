using System;

public abstract class Animal
{
    public string Name { get; set; }
    public int Age { get; set; }
    public string Color { get; set; }
    
    public abstract void MakeSound();
    
    public override string ToString()
    {
        return $"{GetType().Name}: Name={Name}, Age={Age}, Color={Color}";
    }
}

public class Dog : Animal
{
    public string Breed { get; set; }
    
    public override void MakeSound()
    {
        Console.WriteLine($"{Name} says: Woof!");
    }
    
    public override string ToString()
    {
        return base.ToString() + $", Breed={Breed}";
    }
}

public class Cat : Animal
{
    public bool IsIndoor { get; set; }
    
    public override void MakeSound()
    {
        Console.WriteLine($"{Name} says: Meow!");
    }
    
    public override string ToString()
    {
        return base.ToString() + $", IsIndoor={IsIndoor}";
    }
}
