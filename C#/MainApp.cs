using System;
using static System.Console;

{
    class MainApp
    {
        static void Main(string[] args)
        {
            int a = 1;

            /*for (int i = 1; i <= 10; i++)
            {
                WriteLine($"{a} {a+1}");
                a++;
            }
            */

            

            while (a <= 10)
            {
                WriteLine($"{a} {a+1}");
                a++;
            }
            /*

            WriteLine("정수를 입력하세요");
            int? b = int.Parse(ReadLine());
            // 삼항 연산자
            /*string c = (b % 2 == 0) ? "짝수" : "홀수";
            WriteLine(c);
            */
            WriteLine("숫자 비교");
            WriteLine("정수를 입력하세요");

            int? d = int.Parse(ReadLine());
            int? e = int.Parse(ReadLine());


            if ( d > 0 )
            {
                string? f = ( d % 2 == 0 )? $"{d}는 0보다 큰 짝수" : $"{d}는 0보다 큰 홀수";
                WriteLine(f);  
            }
            else
            {
                WriteLine($"{d}는 0보다 작거나 같습니다.");
            }
            
            /*if ( d > e )
            {
                WriteLine("{0} > {1} 는 {2}입니다.", d, e, d>e);
            }           
            else if ( d < e )
            {
                WriteLine("{0} > {1} 는 {2}입니다.", d, e, d>e);
            }
            else
            {
                WriteLine("d와 e는 같습니다.");*/
            
        }
    }
}