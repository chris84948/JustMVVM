###JustMVVM

JustMVVM is *the* simplest ever MVVM framework. Unlike Prism, MVVMLight or Caliburn.Micro, JustMVVM just does 2 things. There's an implementation for INotifyPropertyChanged and for ICommand.

JustMVVM came about because I felt like the bigger frameworks had a little too much going on in them, and I like to know exactly how all this stuff works. I was also getting fed up of copying the same classes into my projects every time I created a new WPF project, so I also put this into NuGet as a package you can download directly.

**MVVMBase** is the base class implementing the INotifyPropertyChanged interface. It can be used like this -

	Public class Card : MVVMBase
	{
		private eSuit _suit;
		public eSuit Suit
		{
			get { return _suit; }
			set 
			{
				_suit = value;
				
				// Any of the following ways work to notify property changed
				OnPropertyChanged("Suit");			// By string name
				OnPropertyChanged(() => Suit);		// By function name
				OnPropertyChanged();				// By CallerMemberFunction
			}
		}
	}
	
That's it, super simple!


**RelayCommand** is the other class in the library and it is a really simple implementation of ICommand. It can be used like this -

	<Button Command="{Binding ShuffleDeckCommand}" />

	public ICommand ShuffleDeckCommand { get { return new RelayCommand(ShuffleDeckExec, CanShuffleDeckExec); } }

	private bool CanShuffleDeckExec()
    {
        return _isGameComplete;
    }

    private void ShuffleDeckExec()
    {
        Deck.Shuffle();
    }

or with using RelayCommand<T>

	<Button Command="{Binding ShuffleDeckCommand}"
			CommandParameter={Binding Deck}/>

	public ICommand ShuffleDeckCommand { get { return new RelayCommand<Deck>(ShuffleDeckExec, CanShuffleDeckExec); } }

	private bool CanShuffleDeckExec(Deck deck)
    {
        return deck != null && _isGameComplete;
    }

    private void ShuffleDeckExec(Deck deck)
    {
        deck.Shuffle();
    }
	

That's it. Hope you like it!
