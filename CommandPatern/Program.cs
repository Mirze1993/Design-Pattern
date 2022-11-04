namespace CommandPatern
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Create user and let her compute

            Account a = new("a", 200);
            Account b = new("b", 300);

            Operation user = new Operation(a,b);
            // User presses calculator buttons
            user.Compute( 100);
            user.Compute(50);
            user.Compute( 10);
            user.Compute( 2);
            // Undo 4 commands
            user.Undo(4);
            // Redo 3 commands
            user.Redo(3);
        }
    }

    public enum OperationType{
        Debit,Credit
    }

    public class Account
    {
         int amount;
         string name;


         public Account(string name,int amount)
         {
             this.name = name;this.amount = amount;
         }

         public void Operation(OperationType type, int operand)
         {
             switch (type)
             {
                 case OperationType.Credit:
                 {
                     if (operand > amount)
                         throw new Exception("operand>amount");
                     amount -= operand;
                 } break;
                 case OperationType.Debit:amount+=operand;break;

                 default: throw new Exception("type is null");
             }

             Console.WriteLine($"{name} amount {amount}");

         }

       
    }
    public abstract class Command
    {
        public abstract void Execute();
        public abstract void UnExecute();
    }

    public class OperationCommand : Command
    {
        
        private int amount;
        private Account fromAccount;
        private Account toAccount;

        public OperationCommand( int amount, Account fromAccount, Account toAccount)
        {
            
            this.amount = amount;
            this.fromAccount = fromAccount;
            this.toAccount = toAccount;
        }

        public override void Execute()
        {
            fromAccount.Operation(OperationType.Credit, amount);
            toAccount.Operation(OperationType.Debit, amount);
        }

        public override void UnExecute()
        {
            fromAccount.Operation(OperationType.Debit, amount);
            toAccount.Operation(OperationType.Credit, amount);
        }
    }


    public class Operation
    {
        // Initializers
        private Account fromAccount;
        private Account toAccount;
        List<Command> commands = new();
        int current = 0;

        public Operation(Account fromAccount, Account toAccount)
        {
            this.fromAccount = fromAccount;
            this.toAccount = toAccount;
        }

        public void Redo(int levels)
        {
            Console.WriteLine("\n---- Redo {0} levels ", levels);
            // Perform redo operations
            for (int i = 0; i < levels; i++)
            {
                if (current < commands.Count - 1)
                {
                    Command command = commands[current++];
                    command.Execute();
                }
            }
        }
        public void Undo(int levels)
        {
            Console.WriteLine("\n---- Undo {0} levels ", levels);

            // Perform undo operations
            for (int i = 0; i < levels; i++)
            {
                if (current > 0)
                {
                    Command command = commands[--current] as Command;
                    command.UnExecute();
                }
            }
        }
        public void Compute(int operand)
        {
            // Create command operation and execute it
            Command command = new OperationCommand(operand, fromAccount, toAccount);
                 command.Execute();
            // Add command to undo list
            commands.Add(command);
            current++;
        }

    }

}