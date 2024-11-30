using System;

namespace _27._11Event
{
    internal class Program
    {
        static void Main(string[] args)
        {
            User user1 = new User("Bill Gates", new Account(21, 123456789, 100));

            user1.account.Event += DisplayM;
            user1.AccountAction += DisplayUs;

            user1.Start(50);
            user1.Put(500);
            user1.Take(900);
            user1.Take(-800);
            user1.Change(12345); 
        }

        
        static void DisplayM(object? sender, AccountEventArgs e)
        {
            Console.WriteLine($"Ответ банка: {e.Message} {e.Sum}. Баланс: {e.Balance} | Срок карты: {e.Strok} | Пин-код: {e.Pin}");
            Console.WriteLine("-------------------------------------------------------------------------");
        }

        
        static void DisplayUs(object? sender, AccountEventArgs e)
        {
            if (sender is User user)
            {
                Console.WriteLine($"Действие пользователя: {e.Message} {e.Sum}. Клиент: {user.Fio}");
            }
        }

        public class Account
        {
            public event EventHandler<AccountEventArgs>? Event;

            public int Strok { get; private set; }
            public int Pin { get; private set; }
            public int Balance { get; private set; }

            public Account(int strok, int pin, int initialBalance)
            {
                Strok = strok;
                Pin = pin;
                Balance = initialBalance > 0 ? initialBalance : 0;
            }

            public void Start(int start)
            {
                Strok = start;
                Event?.Invoke(this, new AccountEventArgs("Старт карты", 0, Strok, Pin, Balance));
            }

            public void Change(int pin)
            {
                if (pin >= 1000 && pin <= 999999) 
                {
                    Pin = pin;
                    Event?.Invoke(this, new AccountEventArgs("Пин-код изменен на", 0, Strok, Pin, Balance));
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    try
                    {
                        Event?.Invoke(this, new AccountEventArgs("Некорректный пин-код", 0, Strok, Pin, Balance));
                    }
                    finally
                    {
                        Console.ResetColor();
                    }
                }
            }

            public void Put(int sum)
            {
                if (sum > 0)
                {
                    Balance += sum;
                    Event?.Invoke(this, new AccountEventArgs("Средства зачислены", sum, Strok, Pin, Balance));
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    try
                    {
                        Event?.Invoke(this, new AccountEventArgs("Невозможно зачислить отрицательную сумму", 0, Strok, Pin, Balance));
                    }
                    finally
                    {
                        Console.ResetColor();
                    }
                }
            }

            public void Take(int sum)
            {
                if (sum > 0)
                {
                    if (Balance >= sum)
                    {
                        Balance -= sum;
                        Event?.Invoke(this, new AccountEventArgs("Средства сняты", sum, Strok, Pin, Balance));
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        try
                        {
                            Event?.Invoke(this, new AccountEventArgs("Недостаточно средств на счете", sum, Strok, Pin, Balance));
                        }
                        finally
                        {
                            Console.ResetColor();
                        }
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    try
                    {
                        Event?.Invoke(this, new AccountEventArgs("Невозможно снять отрицательную сумму", 0, Strok, Pin, Balance));
                    }
                    finally
                    {
                        Console.ResetColor();
                    }
                }
            }
        }

        public class AccountEventArgs : EventArgs
        {
            public AccountEventArgs(string? message, int sum, int strok, int pin, int balance)
            {
                Message = message;
                Sum = sum;
                Strok = strok;
                Pin = pin;
                Balance = balance;
            }

            public string? Message { get; set; }
            public int Sum { get; set; }
            public int Strok { get; set; }
            public int Pin { get; set; }
            public int Balance { get; set; }
        }

        public class User
        {
            public EventHandler<AccountEventArgs>? AccountAction;

            public string? Fio { get; set; }
            public Account account { get; private set; }

            public User(string? fio, Account account)
            {
                Fio = fio;
                this.account = account;
            }

            public void Take(int sum)
            {
                AccountAction?.Invoke(this, new AccountEventArgs("Снятие средств", sum, account.Strok, account.Pin, account.Balance));
                account.Take(sum);
            }

            public void Put(int sum)
            {
                AccountAction?.Invoke(this, new AccountEventArgs("Пополнение счета", sum, account.Strok, account.Pin, account.Balance));
                account.Put(sum);
            }

            public void Start(int start)
            {
                AccountAction?.Invoke(this, new AccountEventArgs("Начало использования карты", 0, start, account.Pin, account.Balance));
                account.Start(start);
            }

            public void Change(int pin)
            {
                AccountAction?.Invoke(this, new AccountEventArgs("Изменение пин-кода", 0, account.Strok, pin, account.Balance));
                account.Change(pin);
            }
        }
    }
}

