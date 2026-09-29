using System;
class program
{
    static void Main()
    {
        Console.WriteLine("Введите ваш возраст");
        String input = Console.ReadLine();
        if (int.TryParse(input, out int age))
        {
            if (age >= 18)
            {
                Console.WriteLine("Можно получить права");
            }
            else if (age < 0)
            {
                Console.WriteLine("не может быть отрацательный возраст");
            }
            else
            {
                int yearsLeft = 18 - age;
                Console.WriteLine("Пока еще рано, подождите и потом идите");
            }
        }
    }
}
    


