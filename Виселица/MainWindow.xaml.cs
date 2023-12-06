using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace HangmanGame
{
    public partial class MainWindow : Window
    {
        private List<Categories> wordCategories;
        private string selectedCategory;
        private string secretWord;
        private List<char> guessedLetters;
        private int maxAttempts = 6;
        private int currentAttempts;
        private int totalScore = 0;
        private int comboBonus = 0;
        private int maxCombo = 0;
        private int hangmanImageIndex = 0;
        string db_name = @"C:\Users\Alena\Desktop\BASD.db";
        public string playerName = null;
        public MainWindow(string playerName)
        {
            InitializeComponent();
            InitializeWordCategories();
            DisplayHighScores(LoadHighScores());
            
            this.playerName = playerName;
        }
        
        private void InitializeWordCategories()
        {
            wordCategories = Categories.GetDefaultCategories();

            // Очистите Items, если они уже есть
            CategoryComboBox.Items.Clear();

            // Используйте Categories.Category для получения списка категорий
            CategoryComboBox.ItemsSource = wordCategories.Select(category => category.Category).ToList();
            CategoryComboBox.SelectionChanged += CategoryComboBox_SelectionChanged;
        }

        private void CategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedCategory = CategoryComboBox.SelectedItem?.ToString();
        }

        private void StartNewGame(List<string> words)
        {
            Random random = new Random();
            secretWord = words[random.Next(words.Count)];

            guessedLetters = new List<char>();
            currentAttempts = 0;
            WordDisplayItemsControl.ItemsSource = null;
            UpdateWordDisplay();
            EnableLetterButtons();
            UpdateHangmanImage();
            CheckGameResult();
        }


        private void UpdateWordDisplay()
        {
            List<string> displayWordList = new List<string>();

            foreach (char letter in secretWord)
            {
                if (guessedLetters.Contains(letter))
                {
                    displayWordList.Add(letter.ToString());
                }
                else
                {
                    displayWordList.Add("_");
                }
            }

            WordDisplayItemsControl.ItemsSource = displayWordList;
            CheckGameResult();
        }

        private void LetterButton_Click(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is Button button && button.Content is string content)
            {
                char letter = content.ToLower()[0];
                CheckLetter(letter);
                button.IsEnabled = false;
            }
        }

        private void CheckLetter(char letter)
        {
            if (!guessedLetters.Contains(letter))
            {
                guessedLetters.Add(letter);

                if (!secretWord.Contains(letter.ToString()))
                {
                    currentAttempts++;
                    hangmanImageIndex++;
                    UpdateHangmanImage();
                 
                    if (currentAttempts == maxAttempts)
                    {
                        SaveGameResult(false);
                    }
                }
                else
                {
                    comboBonus += 5;
                    totalScore += 10 + comboBonus;
                    maxCombo = Math.Max(maxCombo, comboBonus);
                }

                UpdateWordDisplay();
                UpdateScoreText();
                
            }
        }


        private void EnableLetterButtons()
        {
            foreach (UIElement element in GamePanel.Children)
            {
                if (element is Button button)
                {
                    button.IsEnabled = true;
                }
            }
        }

        private void UpdateScoreText()
        {
            ScoreTextBlock.Text = $"Счет: {totalScore} | Макс. комбо: {maxCombo}";
        }

        private void UpdateHangmanImage()
        {
            string imagePath = $"pack://application:,,,/Images/hangman_{hangmanImageIndex}.jpg";
            HangmanImage.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(imagePath, UriKind.Absolute));
        }

        private void SaveGameResult(bool isWinner)
        {
            if (isWinner)
            {
                GameResult result = new GameResult
                {
                    NAME = playerName,
                    RESULT = totalScore,
                };

                List<GameResult> highScores = LoadHighScores();
                highScores.Add(result);
                highScores = highScores.OrderByDescending(r => r.RESULT).Take(10).ToList();

                SaveHighScores(highScores);
                DisplayHighScores(highScores);
            }
            else
            {

            }
        }

        private List<GameResult> LoadHighScores()
        {
            List<GameResult> highScores = new List<GameResult>();

            using (SQLiteConnection con1 = new SQLiteConnection("Data Source=" + db_name + ";Version=3;"))
            {
                con1.Open();
                string sql = "SELECT * FROM RESULTAT";
                using (SQLiteCommand command = new SQLiteCommand(sql, con1))
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        GameResult result = new GameResult
                        {
                            NAME = reader["NAME"].ToString(),
                            RESULT = Convert.ToInt32(reader["RESULT"]),
                        };
                        highScores.Add(result);
                    }
                }
            }

            return highScores;
        }

        private void SaveHighScores(List<GameResult> highScores)
        {
            using (SQLiteConnection con1 = new SQLiteConnection("Data Source=" + db_name + ";Version=3;"))
            {
                con1.Open();
                using (SQLiteCommand clearCommand = new SQLiteCommand("DELETE FROM RESULTAT", con1))
                {
                    clearCommand.ExecuteNonQuery();
                }

                foreach (var result in highScores)
                {
                    using (SQLiteCommand insertCommand = new SQLiteCommand("INSERT INTO RESULTAT (NAME, RESULT) VALUES (@Name, @Result)", con1))
                    {
                        insertCommand.Parameters.AddWithValue("@Name", result.NAME);
                        insertCommand.Parameters.AddWithValue("@Result", result.RESULT);
                        insertCommand.ExecuteNonQuery();
                    }
                }
            }
        }

        private void DisplayHighScores(List<GameResult> highScores)
        {
            datagrid.ItemsSource = highScores;
        }

        private void StartGameButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedCategory != null)
            {
                var selectedCategoryObject = wordCategories.FirstOrDefault(category => category.Category == selectedCategory);
                if (selectedCategoryObject != null)
                {
                    List<string> words = selectedCategoryObject.Words;
                    StartNewGame(words);
                }
            }
            else
            {
                MessageBox.Show("Выберите категорию перед началом игры.");
            }
        }
        private void CheckGameResult()
        {
            if (currentAttempts == maxAttempts)
            {
                MessageBox.Show("Вы, " + playerName + ", к нашему глубочайшему сожалению, проиграли, загаданное слово: " + secretWord);
                SaveGameResult(false); 
                
            }
            else if (!secretWord.Any(letter => !guessedLetters.Contains(letter)))
            {
                MessageBox.Show("Поздравляем! Вы, " + playerName + ", угадали слово: " + secretWord);
                SaveGameResult(true); 
                
            }
        }

        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedCategory != null)
            {
                var selectedCategoryObject = wordCategories.FirstOrDefault(category => category.Category == selectedCategory);
                if (selectedCategoryObject != null)
                {
                    List<string> words = selectedCategoryObject.Words;
                    MainWindow newMainWindow = new MainWindow(playerName);
                    newMainWindow.Show();
                    Close();
                }
            }
            else
            {
                MessageBox.Show("Выберите категорию перед началом игры.");
            }
        }


    }
}