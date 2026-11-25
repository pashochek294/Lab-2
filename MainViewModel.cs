using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace TicTacToeLab
{
    public class CellViewModel : INotifyPropertyChanged
    {
        private string _text = "";
        private bool _isEnabled = true;

        public event PropertyChangedEventHandler? PropertyChanged;

        public int Row { get; set; }
        public int Col { get; set; }
        public ICommand ClickCommand { get; set; }

        public CellViewModel(int row, int col, ICommand clickCommand)
        {
            Row = row;
            Col = col;
            ClickCommand = clickCommand;
        }

        public string Text
        {
            get { return _text; }
            set { _text = value; OnPropertyChanged("Text"); }
        }

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { _isEnabled = value; OnPropertyChanged("IsEnabled"); }
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class MainViewModel : INotifyPropertyChanged
    {
        private const string TurnMessage = "Ваш хід (X)";
        private const string HumanWinMessage = "Перемога! Ви виграли (X)!";
        private const string ComputerWinMessage = "Комп'ютер переміг (O)!";
        private const string DrawMessage = "Нічия! Вільних клітинок немає.";

        private readonly TicTacToeGame _game = new TicTacToeGame();
        private int _selectedLevelIndex = 2;
        private string _statusText = TurnMessage;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<CellViewModel> Cells { get; } = new ObservableCollection<CellViewModel>();
        public ICommand NewGameCommand { get; }
        public ICommand CellClickCommand { get; }

        public MainViewModel()
        {
            NewGameCommand = new RelayCommand(StartNewGame);
            CellClickCommand = new RelayCommand(OnCellClicked);

            for (int r = 0; r < TicTacToeGame.BoardSize; r++)
            {
                for (int c = 0; c < TicTacToeGame.BoardSize; c++)
                {
                    Cells.Add(new CellViewModel(r, c, CellClickCommand));
                }
            }

            UpdateBoardView();
        }

        public int SelectedLevelIndex
        {
            get { return _selectedLevelIndex; }
            set
            {
                _selectedLevelIndex = value;
                OnPropertyChanged("SelectedLevelIndex");
            }
        }

        public string StatusText
        {
            get { return _statusText; }
            set
            {
                _statusText = value;
                OnPropertyChanged("StatusText");
            }
        }

        private void StartNewGame(object? parameter)
        {
            _game.Reset();
            StatusText = TurnMessage;
            UpdateBoardView();
        }

        private void OnCellClicked(object? parameter)
        {
            CellViewModel? cell = parameter as CellViewModel;
            if (cell == null)
            {
                return;
            }

            bool moved = _game.MakeMove(cell.Row, cell.Col, TicTacToeGame.HumanMark);
            if (!moved)
            {
                return;
            }

            if (!_game.IsGameOver)
            {
                int currentLevel = _selectedLevelIndex + 1;
                MovePosition? computerMove = _game.ChooseComputerMove(currentLevel);
                if (computerMove != null)
                {
                    _game.MakeMove(computerMove.Row, computerMove.Col, TicTacToeGame.ComputerMark);
                }
            }

            UpdateStatusMessage();
            UpdateBoardView();
        }

        private void UpdateStatusMessage()
        {
            if (_game.Winner == TicTacToeGame.HumanMark)
            {
                StatusText = HumanWinMessage;
            }
            else if (_game.Winner == TicTacToeGame.ComputerMark)
            {
                StatusText = ComputerWinMessage;
            }
            else if (_game.IsDraw())
            {
                StatusText = DrawMessage;
            }
            else
            {
                StatusText = TurnMessage;
            }
        }

        private void UpdateBoardView()
        {
            int index = 0;
            for (int r = 0; r < TicTacToeGame.BoardSize; r++)
            {
                for (int c = 0; c < TicTacToeGame.BoardSize; c++)
                {
                    char mark = _game.GetCell(r, c);
                    if (mark == TicTacToeGame.EmptyCell)
                    {
                        Cells[index].Text = "";
                    }
                    else
                    {
                        Cells[index].Text = mark.ToString();
                    }

                    Cells[index].IsEnabled = (mark == TicTacToeGame.EmptyCell) && (!_game.IsGameOver);
                    index++;
                }
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
