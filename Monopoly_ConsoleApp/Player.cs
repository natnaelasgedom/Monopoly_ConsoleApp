using System;
using System.Collections.Generic;

namespace Monopoly_ConsoleApp
{
    public class Player
    {
        public int Balance { get; set; }
        public bool InJail { get; set; }
        public bool MyTurn { get; set; }
        public Property CurrentLocation { get; set; }
        public List<Property> Board { get; set; }
        public List<Property> MyProperties { get; set; }

        public Player(int startCap, List<Property> board)
        {
            Balance = startCap;
            InJail = false;
            MyTurn = false;
            Board = board;
            CurrentLocation = Board[0];
            MyProperties = new List<Property>();

        }

        private int ThrowDice()
        {
            if (!MyTurn)
            {
                return 0;
            }
            return new Random().Next(1, 7);  
        }

        public void MovePlayer()
        {
            var dice = ThrowDice();
            if(dice>0)
            {
                var currentLocIndex = Board.FindIndex(x => x.Name == CurrentLocation.Name);
                CurrentLocation = currentLocIndex + dice < Board.Count ? Board[currentLocIndex + dice] : Board[currentLocIndex + dice - Board.Count];
                dice = 0;
                CalculateRent();
            }     
        }

        private void CalculateRent()
        {
            if (!MyProperties.Contains(CurrentLocation) && CurrentLocation.Owner != null)
            {
                Balance -= CurrentLocation.Rent;
                CurrentLocation.Owner.Balance += CurrentLocation.Rent;
            }
        }

        public bool BuyProperty()
        {
            if (IsAvailable())
            {
                Balance -= CurrentLocation.Price;
                CurrentLocation.Owner = this;
                MyProperties.Add(CurrentLocation);
                return true;

            }
            return false;
        }

        

        //Method to sell property. My balance is increased and deletes the property from my list. The buyer's balance is decreased and the property added to buyer's list.
        //The property's owner is set to the buyer.
        public bool SellProperty(Player buyer, Property propToSell)
        {
            if (IsForSale(buyer, propToSell))
            {
                Balance += propToSell.Price;
                MyProperties.Remove(propToSell);
                buyer.Balance -= propToSell.Price;
                buyer.MyProperties.Add(propToSell);
                propToSell.Owner = buyer;
                return true;
            }

            return false;
        }

        //Check to see if there is no owner and that I can afford it.
        private bool IsAvailable()
        {
            return (CurrentLocation.Price <= Balance && CurrentLocation.Owner == null) ? true : false;
        }

        //Check if you own the place and if the buyer can afford it
        private bool IsForSale(Player buyer, Property propToSell)
        {
            return (MyProperties.Contains(propToSell) && buyer.Balance >= propToSell.Price) ? true : false;
        }
    }
}