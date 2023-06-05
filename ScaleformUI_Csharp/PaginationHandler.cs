﻿namespace ScaleformUI
{
    internal class PaginationHandler
    {
        private int _currentPageIndex;
        private int _currentMenuIndex;
        private int currentPage;
        private int itemsPerPage;
        private int minItem;
        private int maxItem;
        private int totalItems;
        private int scaleformIndex;

        internal ScrollingType scrollType = ScrollingType.Classic;
        internal int CurrentPage { get => currentPage; set => currentPage = value; }
        internal int ItemsPerPage { get => itemsPerPage; set => itemsPerPage = value; }
        internal int TotalItems { get => totalItems; set => totalItems = value; }
        internal int TotalPages => (int)Math.Floor(totalItems / (float)itemsPerPage);
        internal int CurrentPageStartIndex => CurrentPage * itemsPerPage;
        internal int CurrentPageEndIndex
        {
            get
            {
                if (totalItems > itemsPerPage)
                {
                    if (totalItems % itemsPerPage == 0)
                    {
                        return CurrentPageStartIndex + itemsPerPage - 1;
                    }
                    else
                    {
                        if (currentPage == TotalPages)
                            return CurrentPageStartIndex + GetPageIndexFromMenuIndex(totalItems - 1);
                        else
                            return CurrentPageStartIndex + itemsPerPage - 1;

                    }
                }
                else
                {
                    return totalItems;
                }
            }
        }
        internal int CurrentPageIndex { get => _currentPageIndex; set => _currentPageIndex = GetPageIndexFromMenuIndex(value); }
        internal int CurrentMenuIndex { get => _currentMenuIndex; set => _currentMenuIndex = value; }
        internal int MinItem { get => minItem; set => minItem = value; }
        internal int MaxItem { get => maxItem; set => maxItem = value; }
        internal int ScaleformIndex { get => scaleformIndex; set => scaleformIndex = value; }

        internal bool IsItemVisible(int menuIndex)
        {
            return menuIndex >= minItem || menuIndex <= minItem && menuIndex <= maxItem;
        }

        internal int GetScaleformIndex(int menuIndex)
        {
            int id = 0;
            if (minItem <= menuIndex)
            {
                id = menuIndex - minItem;
            }
            else if (minItem > menuIndex && maxItem >= menuIndex)
            {
                id = (menuIndex - maxItem) + (itemsPerPage - 1);
            }
            return id + GetMissingItems();
        }

        internal int GetMenuIndexFromScaleformIndex(int scaleformIndex)
        {
            int id = 0;
            if (minItem <= scaleformIndex)
            {
                id = scaleformIndex + minItem;
            }
            else if (minItem > scaleformIndex && maxItem >= scaleformIndex)
            {
                id = GetMenuIndexFromPageIndex(0, (TotalItems - minItem) - scaleformIndex);
            }
            return id;
        }

        internal int GetPageIndexFromMenuIndex(int menuIndex)
        {
            int page = GetPage(menuIndex);
            int startIndex = page * itemsPerPage;
            return menuIndex - startIndex;
        }

        internal int GetMenuIndexFromPageIndex(int page, int index)
        {
            int initialIndex = page * itemsPerPage;
            return initialIndex + index;
        }

        internal int GetPage(int menuIndex)
        {
            return (int)Math.Floor(menuIndex / (float)itemsPerPage);
        }

        internal int GetPageItemsCount(int page)
        {
            int minItem = page * itemsPerPage;
            int maxItem = minItem + itemsPerPage - 1;
            if (maxItem >= totalItems)
                maxItem = totalItems - 1;
            return (maxItem - minItem) + 1;
        }

        internal int GetMissingItems()
        {
            int count = GetPageItemsCount(currentPage);
            return itemsPerPage - count;
        }

        internal bool GoUp()
        {
            bool overflow = false;
            CurrentMenuIndex--;
            if (CurrentMenuIndex < 0)
            {
                CurrentMenuIndex = TotalItems - 1;
                overflow = true;
            }
            CurrentPageIndex = CurrentMenuIndex;
            ScaleformIndex--;
            CurrentPage = GetPage(CurrentMenuIndex);
            if (ScaleformIndex < 0)
            {
                if (TotalItems <= itemsPerPage)
                {
                    ScaleformIndex = TotalItems - 1;
                    return false;
                }
                if (scrollType == ScrollingType.Infinite || (scrollType == ScrollingType.Classic && !overflow))
                {
                    minItem--;
                    maxItem--;
                    if (minItem < 0)
                        minItem = TotalItems - 1;
                    if (maxItem < 0)
                        maxItem = TotalItems - 1;
                    ScaleformIndex = 0;
                    return true;
                }
                else if (scrollType == ScrollingType.Paginated || (scrollType == ScrollingType.Classic && overflow))
                {
                    minItem = CurrentPageStartIndex;
                    maxItem = CurrentPageEndIndex;
                    ScaleformIndex = GetPageIndexFromMenuIndex(CurrentPageEndIndex);
                    if (scrollType == ScrollingType.Classic)
                    {
                        int missingItems = GetMissingItems();
                        ScaleformIndex += missingItems;
                    }
                    return true;
                }
            }
            return false;
        }

        internal bool GoDown()
        {
            bool overflow = false;
            CurrentMenuIndex++;
            if (CurrentMenuIndex >= TotalItems)
            {
                CurrentMenuIndex = 0;
                overflow = true;
            }
            CurrentPageIndex = CurrentMenuIndex;
            ScaleformIndex++;
            if (ScaleformIndex >= totalItems)
            {
                ScaleformIndex = 0;
                return false;
            }
            else if (scaleformIndex > itemsPerPage - 1)
            {
                if (scrollType == ScrollingType.Infinite || (scrollType == ScrollingType.Classic && !overflow))
                {
                    CurrentPage = GetPage(CurrentMenuIndex);
                    ScaleformIndex = itemsPerPage - 1;
                    minItem++;
                    maxItem++;
                    if (minItem >= totalItems)
                        minItem = 0;
                    if (maxItem >= totalItems)
                        maxItem = 0;
                    return true;
                }
                else if (scrollType == ScrollingType.Paginated || (scrollType == ScrollingType.Classic && overflow))
                {
                    CurrentPage = GetPage(CurrentMenuIndex);
                    minItem = CurrentPageStartIndex;
                    maxItem = CurrentPageEndIndex;
                    ScaleformIndex = 0;
                    return true;
                }
            }
            CurrentPage = GetPage(CurrentMenuIndex);
            return false;
        }

        public override string ToString()
        {
            string returned = "";
            returned += "CurrentPageStartIndex:" + CurrentPageStartIndex + "\n";
            returned += "CurrentPageEndIndex:" + CurrentPageEndIndex + "\n";
            returned += "GetPageIndexFromMenuIndex(CurrentPageStartIndex):" + GetPageIndexFromMenuIndex(CurrentPageStartIndex) + "\n";
            returned += "GetPageIndexFromMenuIndex(CurrentPageEndIndex):" + GetPageIndexFromMenuIndex(CurrentPageEndIndex) + "\n";
            returned += "GetPageIndexFromMenuIndex(totalItems):" + GetPageIndexFromMenuIndex(totalItems) + "\n";
            returned += "ScaleformIndex:" + ScaleformIndex + "\n";
            returned += "TotalPages:" + TotalPages + "\n";
            returned += "_currentPageIndex: " + _currentPageIndex + "\n";
            returned += "_currentMenuIndex: " + _currentMenuIndex + "\n";
            returned += "currentPage: " + currentPage + "\n";
            returned += "itemsPerPage: " + itemsPerPage + "\n";
            returned += "minItem: " + minItem + "\n";
            returned += "maxItem: " + maxItem + "\n";
            returned += "totalItems: " + totalItems + "\n";
            returned += "scaleformIndex: " + scaleformIndex + "\n";
            returned += "/////////////////////////////////////////////\n";
            return returned;
        }
    }
}
