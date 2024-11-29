﻿using Allup.Application.ViewModels;
using Allup.Domain.Entities;

namespace Allup.Application.Services.Abstracts;

public interface IWishlistService : ICrudService<WishlistViewModel, Wishlist, WishlistCreateViewModel>
{

}

