using System;
using System.Windows;

namespace HangmanGame
{
    public partial class StartWindow : Window
    {
        // ...
        public event EventHandler<string> NameEntered;


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                // Передаем имя в основное окно через событие
                NameEntered?.Invoke(this, NameTextBox.Text);
                MainWindow mainWindow = new MainWindow(NameTextBox.Text);
                mainWindow.Show();
                // Закрываем текущее стартовое окно
                Close();
                
            }
            else
            {
                MessageBox.Show("Пожалуйста, введите ваше имя перед началом игры.");
            }
        }
    }
}
