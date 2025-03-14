// Biến để lưu từ khóa tìm kiếm và trang hiện tại
let currentSearch = '';
let currentPage = 1;

// Hàm tìm kiếm bằng AJAX
function searchMenuItems(searchTerm) {
    currentSearch = searchTerm || currentSearch;
    var urlElement = document.getElementById('searchMenuItemsUrl'); // Lấy phần tử
    if (!urlElement) {
        console.error('Element with id "searchMenuItemsUrl" not found.');
        return;
    }
    var url = urlElement.dataset.url; // Lấy URL từ dataset
    $.ajax({
        url: url,
        type: 'GET',
        data: { search: currentSearch, page: currentPage },
        success: function (data) {
            updateMenuItemsTable(data);
        },
        error: function (xhr, status, error) {
            console.error('Error fetching menu items:', error);
            toastr.error('Có lỗi xảy ra khi tìm kiếm món ăn.', 'Lỗi!', { timeOut: 3000 });
        }
    });
}

// Hàm cập nhật bảng và phân trang
function updateMenuItemsTable(data) {
    var tbody = $('#menuItemsTable');
    var noResultsMessage = $('#noResultsMessage');
    var pagination = $('#pagination');
    tbody.empty();

    if (data.items.length > 0) {
        data.items.forEach(item => {
            var row = `
                <tr class="menu-item">
                    <td class="item-name">${item.itemName}</td>
                    <td><strong class="text-success">${item.price.toLocaleString()}₫</strong></td>
                    <td>
                        <img src="${item.imageUrl}" alt="${item.itemName}" class="img-thumbnail" style="width: 80px; height: 80px; object-fit: cover;">
                    </td>
                    <td class="item-status">
                        ${item.status ? '<span class="badge bg-success">Có sẵn</span>' : '<span class="badge bg-danger">Hết hàng</span>'}
                    </td>
                    <td class="item-category">${item.categoryName}</td>
                   <td>
   <a href="/Admin/Menu/Edit/${item.itemId}" class="btn btn-sm btn-primary">
<a href="/Admin/Menu/Details/${item.itemId}" class="btn btn-sm btn-info">
<a href="/Admin/Menu/Delete/${item.itemId}" class="btn btn-sm btn-danger btn-delete">
</td>

                </tr>`;
            tbody.append(row);
        });
        noResultsMessage.hide();
    } else {
        tbody.append('<tr><td colspan="6" class="text-center text-danger">Không tìm thấy món ăn nào phù hợp.</td></tr>');
        noResultsMessage.show();
    }

    updatePagination(data);
}

// Hàm cập nhật phân trang
function updatePagination(data) {
    var pagination = $('#pagination');
    pagination.empty();

    var pageCount = data.pageCount;
    var pageNumber = data.pageNumber;
    var hasPreviousPage = data.hasPreviousPage;
    var hasNextPage = data.hasNextPage;

    var paginationHtml = '<ul class="pagination">';

    paginationHtml += `<li class="page-item ${!hasPreviousPage ? 'disabled' : ''}">
        <a class="page-link" href="#" onclick="goToPage(${pageNumber - 1}); return false;" ${!hasPreviousPage ? 'aria-disabled="true"' : ''}>Previous</a>
    </li>`;

    var startPage = Math.max(1, pageNumber - 2);
    var endPage = Math.min(pageCount, pageNumber + 2);

    if (startPage > 1) {
        paginationHtml += '<li class="page-item"><a class="page-link" href="#" onclick="goToPage(1); return false;">1</a></li>';
        if (startPage > 2) {
            paginationHtml += '<li class="page-item disabled"><span class="page-link">...</span></li>';
        }
    }

    for (var i = startPage; i <= endPage; i++) {
        paginationHtml += `<li class="page-item ${i === pageNumber ? 'active' : ''}">
            <a class="page-link" href="#" onclick="goToPage(${i}); return false;">${i}</a>
        </li>`;
    }

    if (endPage < pageCount) {
        if (endPage < pageCount - 1) {
            paginationHtml += '<li class="page-item disabled"><span class="page-link">...</span></li>';
        }
        paginationHtml += `<li class="page-item"><a class="page-link" href="#" onclick="goToPage(${pageCount}); return false;">${pageCount}</a></li>`;
    }

    paginationHtml += `<li class="page-item ${!hasNextPage ? 'disabled' : ''}">
        <a class="page-link" href="#" onclick="goToPage(${pageNumber + 1}); return false;" ${!hasNextPage ? 'aria-disabled="true"' : ''}>Next</a>
    </li>`;

    paginationHtml += '</ul>';
    pagination.html(paginationHtml);
}

// Hàm chuyển trang
function goToPage(page) {
    currentPage = page;
    searchMenuItems();
}

// Hàm xóa tìm kiếm
function clearSearch() {
    $('#searchInput').val('');
    currentSearch = '';
    currentPage = 1;
    searchMenuItems();
}

// Khởi tạo khi trang tải
$(document).ready(function () {
    var successMessageElement = document.getElementById('successMessage');
    var errorMessageElement = document.getElementById('errorMessage');

    if (!successMessageElement || !errorMessageElement) {
        console.error('SuccessMessage or ErrorMessage element not found.');
        return;
    }

    var successMessage = successMessageElement.dataset.value;
    var errorMessage = errorMessageElement.dataset.value;

    if (successMessage) {
        toastr.success(successMessage, 'Thành công!', { timeOut: 3000 });
    }
    if (errorMessage) {
        toastr.error(errorMessage, 'Lỗi!', { timeOut: 3000 });
    }

    // Gọi tìm kiếm lần đầu để tải dữ liệu ban đầu
    searchMenuItems();
});