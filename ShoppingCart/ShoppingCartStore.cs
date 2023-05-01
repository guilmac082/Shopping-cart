﻿using System.Runtime.Intrinsics.X86;
using System;

namespace ShoppingCart.ShoppingCart
{
    public interface IShoppingCartStore
    {
        ShoppingCart Get(int userId);
        void Save(ShoppingCart shoppingCart);
    }

    public class ShoppingCartStore : IShoppingCartStore
    {
        private static readonly Dictionary<int, ShoppingCart> Database = new Dictionary<int, ShoppingCart>();

        public ShoppingCart Get(int userId)
        {
            if (Database.ContainsKey(userId))
                return Database[userId];

            else return new ShoppingCart(userId);
        }

        public void Save(ShoppingCart shoppingCart)
        {
            Database[shoppingCart.UserId] = shoppingCart;
        }
    }
}
