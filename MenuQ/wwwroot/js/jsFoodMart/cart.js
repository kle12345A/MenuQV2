document.addEventListener("DOMContentLoaded", function () {
    const cartItemsContainer = document.getElementById("cart-items");
    const cartCount = document.getElementById("cart-count");
    const totalPriceElement = document.getElementById("total-price");

    let cart = JSON.parse(localStorage.getItem("cart")) || [];

    function saveCart() {
        localStorage.setItem("cart", JSON.stringify(cart));
    }

    function updateCartUI() {
        cartItemsContainer.innerHTML = "";
        let totalPrice = 0;
        let totalItems = 0;

        cart.forEach((item, index) => {
            const listItem = document.createElement("li");
            listItem.classList.add("list-group-item", "d-flex", "justify-content-between", "align-items-center", "lh-sm");
            listItem.innerHTML = `
                <div>
                    <h6 class="my-0">${item.name}</h6>
                    <small class="text-muted">${item.description}</small>
                </div>
                <div class="d-flex align-items-center">
                    <div class="input-group product-qty me-3" style="min-width: 120px;">
                        <button type="button" class="btn btn-danger btn-sm quantity-left-minus" data-index="${index}">
                            <svg width="16" height="16"><use xlink:href="#minus"></use></svg>
                        </button>
                        <input type="text" class="form-control text-center input-number" value="${item.quantity}" style="width: 50px;" disabled>
                        <button type="button" class="btn btn-success btn-sm quantity-right-plus" data-index="${index}">
                            <svg width="16" height="16"><use xlink:href="#plus"></use></svg>
                        </button>
                    </div>
                    <span class="text-body-secondary me-3">${item.price * item.quantity}</span>
                    <button class="btn btn-sm btn-outline-danger remove-item" data-index="${index}">
                        <svg width="24" height="24"><use xlink:href="#trash"></use></svg>
                    </button>
                </div>
            `;
            cartItemsContainer.appendChild(listItem);

            totalPrice += item.price * item.quantity;
            totalItems += item.quantity;
        });

        const totalItem = document.createElement("li");
        totalItem.classList.add("list-group-item", "d-flex", "justify-content-between");
        totalItem.innerHTML = `<span>Tổng: (VND)</span><strong>${totalPrice}</strong>`;
        cartItemsContainer.appendChild(totalItem);

        cartCount.textContent = totalItems;
        totalPriceElement.textContent = `${totalPrice}`;

        saveCart();
    }

    function addToCart(id, name, price, description = "No description") {
        const existingItem = cart.find(item => item.id === id);
        if (existingItem) {
            existingItem.quantity++;
        } else {
            cart.push({ id, name, price, quantity: 1, description });
        }
        updateCartUI();
    }

    cartItemsContainer.addEventListener("click", function (event) {
        const index = event.target.closest("button")?.getAttribute("data-index");
        if (index !== null) {
            if (event.target.closest(".quantity-left-minus")) {
                cart[index].quantity = Math.max(1, cart[index].quantity - 1);
            } else if (event.target.closest(".quantity-right-plus")) {
                cart[index].quantity++;
            } else if (event.target.closest(".remove-item")) {
                cart.splice(index, 1);
            }
            updateCartUI();
        }
    });

    document.querySelectorAll(".product-item .nav-link").forEach(button => {
        button.addEventListener("click", function (event) {
            event.preventDefault();
            const productItem = this.closest(".product-item");
            const name = productItem.querySelector("h3").textContent;
            const price = parseFloat(productItem.querySelector(".price").textContent.replace("$", ""));
            const description = productItem.querySelector(".qyt")?.textContent || "No description";
            const id = parseInt(this.getAttribute("data-id"), 10);
            addToCart(id, name, price, description);
        });
    });

    $("#order-button").click(function () {
        if (cart.length === 0) {
            alert("Giỏ hàng trống!");
            return;
        }
        console.log("Dữ liệu gửi đi:", cart);

        $.ajax({
            url: "/MenuOrder/PlaceOrder", // Đường dẫn đến controller xử lý đơn hàng
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(cart),
            success: function (response) {
                alert("Đặt món thành công!");
                cart = []; // Xóa giỏ hàng sau khi đặt món thành công
                updateCartUI(); // Cập nhật lại giao diện
            },
            error: function (xhr, status, error) {
                alert("Có lỗi xảy ra khi đặt món, vui lòng thử lại!");
                console.error(error);
            }
        });
    });

    updateCartUI();
});