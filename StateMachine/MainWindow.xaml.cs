using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;

namespace StateMachine
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //private async Task<string> ExampleTaskAsync(int Second)
        //{
        //    await Task.Delay(TimeSpan.FromSeconds(Second));

        //    return $"It's Async Completed in {Second} seconds";
        //}

        //private async void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    var result = await ExampleTaskAsync(2);

        //    textBlock.Text = result;
        //}

        private Task<string> ExampleTaskAsync(int Second)
        {
            var stateMachine = new ExampleTaskStateMachine();

            stateMachine.State = -1;

            stateMachine.Builder = AsyncTaskMethodBuilder<string>.Create();

            stateMachine.Second = Second;

            stateMachine.Window = this;

            stateMachine.Builder.Start(ref stateMachine);

            return stateMachine.Builder.Task;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var stateMachine = new ButtonClickStateMachine();

            stateMachine.State = -1;

            stateMachine.Builder = AsyncTaskMethodBuilder.Create();

            stateMachine.Sender = sender;

            stateMachine.E = e;

            stateMachine.Window = this;

            stateMachine.Builder.Start(ref stateMachine);
        }

        internal class ExampleTaskStateMachine : IAsyncStateMachine
        {
            public int State;
            public AsyncTaskMethodBuilder<string> Builder;
            public int Second;
            public MainWindow Window;
            private TaskAwaiter Awaiter;

            public void MoveNext()
            {
                string result = null;

                try
                {
                    if (State != 0)
                    {
                        var task = Task.Delay(Second);
                        
                        Awaiter = task.GetAwaiter();

                        if (!Awaiter.IsCompleted)
                        {
                            State = 0;

                            var _this = this;

                            Builder.AwaitUnsafeOnCompleted(ref Awaiter, ref _this);

                            return;
                        }
                    }

                    State = -1;

                    Awaiter.GetResult();

                    result = String.Format("It's Async Completed in {0} seconds", Second);
                }
                catch (Exception e)
                {
                    State = -2;

                    Builder.SetException(e);
                }

                State = -2;

                Builder.SetResult(result);
            }

            public void SetStateMachine(IAsyncStateMachine stateMachine)
            {
                Builder.SetStateMachine(stateMachine);
            }
        }

        internal class ButtonClickStateMachine : IAsyncStateMachine
        {
            public int State;
            public AsyncTaskMethodBuilder Builder;
            public object Sender;
            public RoutedEventArgs E;
            public MainWindow Window;
            private TaskAwaiter<string> Awaiter;

            public void MoveNext()
            {
                string result = null;

                try
                {
                    if (State != 0)
                    {
                        var task = Window.ExampleTaskAsync(2000);

                        Awaiter = task.GetAwaiter();

                        if (!Awaiter.IsCompleted)
                        {
                            State = 0;

                            var _this = this;

                            Builder.AwaitUnsafeOnCompleted(ref Awaiter, ref _this);

                            return;
                        }
                    }

                    State = -1;

                    result = Awaiter.GetResult();

                    Window.textBlock.Text = result;
                }
                catch (Exception e)
                {
                    State = -2;

                    Builder.SetException(e);
                }

                State = -2;

                Builder.SetResult();
            }

            public void SetStateMachine(IAsyncStateMachine stateMachine)
            {
                Builder.SetStateMachine(stateMachine);
            }
        }
    }
}
