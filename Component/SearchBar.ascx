<%@ Control Language="C#" AutoEventWireup="true" 
    CodeBehind="SearchBar.ascx.cs" 
    Inherits="FruitUnitedMobile.Component.SearchBar" %>
<div class="search-container">
    <div class="search-wrapper">
        <div class="search-icon">
            <i class="fa fa-search"></i>
        </div>
        <asp:TextBox ID="txtSearch" runat="server" 
            CssClass="search-input" 
            placeholder="Search..."
            onkeyup="performSearch(this.value, this.getAttribute('data-target'))"></asp:TextBox>
        <button type="button" class="clear-search-btn" onclick="clearSearchBar(this)" style="display:none;">
            <i class="fa fa-times-circle"></i>
        </button>
    </div>
    <div class="search-info">
        <i class="fa fa-info-circle"></i>
        <asp:Label ID="lblSearchInfo" runat="server" Text="Type to search"></asp:Label>
    </div>
</div>

<style>
    .search-container {
        background: #f8f9fa;
        padding: 15px;
        border-radius: 10px;
        margin-bottom: 20px;
        border: 1px solid #dee2e6;
    }

    .search-wrapper {
        position: relative;
        display: flex;
        align-items: center;
        background: white;
        border: 2px solid #ced4da;
        border-radius: 10px;
        transition: all 0.3s ease;
        overflow: hidden;
    }

    .search-wrapper:focus-within {
        border-color: #007bff !important;
        box-shadow: 0 0 0 0.25rem rgba(0,123,255,0.25);
    }

    .search-icon {
        padding: 0 15px;
        color: #6c757d;
        font-size: 1.1rem;
        pointer-events: none;
    }

    /* THIS IS THE KEY: Remove ALL Bootstrap borders & outlines */
    .search-input {
        flex: 1;
        border: none !important;
        outline: none !important;
        box-shadow: none !important;
        padding: 14px 10px;
        font-size: 1rem;
        background: transparent;
    }

    .clear-search-btn {
        padding: 8px 15px;
        background: transparent;
        border: none;
        color: #6c757d;
        cursor: pointer;
        font-size: 1.3rem;
        transition: color 0.2s;
    }

    .clear-search-btn:hover {
        color: #dc3545;
    }

    .search-info {
        margin-top: 10px;
        font-size: 0.9rem;
        color: #6c757d;
    }

    .search-info i {
        margin-right: 5px;
    }

    /* Show clear button only when typing */
    .search-input:not(:placeholder-shown) ~ .clear-search-btn,
    .search-input:focus ~ .clear-search-btn {
        display: block !important;
    }
</style>

<script type="text/javascript">
    function performSearch(searchText, targetAttribute) {
        searchText = searchText.toLowerCase().trim();
        var searchFields = targetAttribute ? targetAttribute.split(',').map(f => f.trim()) : ['data-abbreviation'];
        var items = document.querySelectorAll('[data-searchable="true"]');
        var visibleCount = 0;

        items.forEach(function (item) {
            var shouldShow = searchText === '';
            if (!shouldShow) {
                searchFields.forEach(function (field) {
                    var value = item.getAttribute(field);
                    if (value && value.toLowerCase().includes(searchText)) {
                        shouldShow = true;
                    }
                });
            }

            if (shouldShow) {
                item.classList.remove('hidden-search');
                visibleCount++;
            } else {
                item.classList.add('hidden-search');
            }
        });

        var noResults = document.querySelectorAll('[data-no-results="true"]');
        var container = document.querySelector('[data-results-container="true"]');

        if (visibleCount === 0 && searchText !== '') {
            noResults.forEach(el => { el.style.display = 'block'; el.textContent = 'No results found for "' + searchText + '"'; });
            if (container) container.style.display = 'none';
        } else {
            noResults.forEach(el => el.style.display = 'none');
            if (container) container.style.display = 'grid';
        }
    }

    function clearSearchBar(button) {
        var wrapper = button.closest('.search-wrapper');
        var input = wrapper.querySelector('.search-input');
        if (input) {
            input.value = '';
            input.focus();
            performSearch('', input.getAttribute('data-target'));
        }
    }

    document.addEventListener('DOMContentLoaded', function () {
        var input = document.getElementById('<%= txtSearch.ClientID %>');
        if (!input) return;

        var container = input.closest('.search-container');
        if (!container) return;

        var modal = container.closest('.modal');
        if (!modal) return;

        modal.addEventListener('shown.bs.modal', function () {
            setTimeout(function () {
                input.value = '';
                performSearch('', input.getAttribute('data-target') || 'data-abbreviation');

                // Hide clear button
                var clearBtn = container.querySelector('.clear-search-btn');
                if (clearBtn) clearBtn.style.display = 'none';

                // Optional: focus for better UX
                input.focus();
            }, 150);
        });
    });
</script>