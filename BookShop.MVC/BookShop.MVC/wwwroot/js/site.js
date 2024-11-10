// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).ready(function () {
    // Настройки Toastr
    toastr.options = {
        positionClass: "toast-bottom-right",
        timeOut: 3000,
        closeButton: true,
        progressBar: true,
    };

    $('.btn_addToCart').on('click', function (e) {
        e.preventDefault();

        var productId = $(this).data('product-id');
        var price = $(this).data('price');
        var bookTitle = $(this).data('book-title');
        var BookId = $(this).data('book-id');
        var AuthorName = $(this).data('author-name');
        var imageStr = $(this).data('image-str');

        var dataToSend = {
            ProductId: productId,
            BookId: BookId,
            Price: price,
            Title: bookTitle,
            AuthorName: AuthorName,
            ImageStr: imageStr
        };

        var jsonData = JSON.stringify(dataToSend);
        console.log(jsonData);

        $.ajax({
            type: "POST",
            url: '/Cart/AddToCart',
            contentType: "application/json; charset=utf-8",
            data: jsonData,
            success: function (response) {
                if (response.success) {
                    $('.cart-count').text(response.cartItemCount);
                    toastr.success('Товар добавлен в корзину! Количество товаров в корзине: ' + response.cartItemCount);
                    updateSubtotal(); // Обновляем subtotal после добавления товара
                } else {
                    toastr.error('Произошла ошибка при добавлении товара в корзину.');
                }
            },
            error: function () {
                toastr.error('Не удалось добавить товар в корзину.');
            }
        });
    });

    $('.btn-remove').on('click', function (e) {
        e.preventDefault();
        var productId = $(this).data('product-id');

        $.ajax({
            type: "POST",
            url: '/Cart/RemoveFromCart',
            data: { productId: productId },
            success: function (response) {
                location.reload(); // Перезагружаем страницу, чтобы отобразить изменения
            },
            error: function () {
                alert('Не удалось удалить товар из корзины.');
            }
        });
    });

    $('#clearCartBtn').on('click', function (e) {
        e.preventDefault();

        $.ajax({
            type: "POST",
            url: '/Cart/ClearCart',
            success: function (response) {
                location.reload(); // Перезагружаем страницу, чтобы отобразить изменения
            },
            error: function () {
                alert('Не удалось очистить корзину.');
            }
        });
    });

    function updateSubtotal() {
        let subtotal = 0;

        $('.cart-item').each(function () {
            const price = parseFloat($(this).find('.item-price').text());
            const quantity = parseInt($(this).find('.item-quantity').text());

            subtotal += price * quantity;
        });

        // Обновляем значение Subtotal на странице
        $('#subtotal-amount').text(`${subtotal.toFixed(2)}€`);
    }

    $('.btn-update-quantity').on('click', function (e) {
        e.preventDefault();

        var productId = $(this).data('product-id');
        var newQuantity = parseInt($(`#quantity-${productId}`).text());
        var updatedQuantity = $(this).hasClass('btn-increase') ? newQuantity + 1 : newQuantity - 1;

        // Проверяем, что количество не меньше 0
        if (updatedQuantity < 0) {
            toastr.error('Количество не может быть меньше 0.');
            return;
        }

        var price = parseFloat($(`#price-${productId}`).text());

        var requestData = {
            productId: productId,
            newQuantity: updatedQuantity
        };

        if (updatedQuantity === 0) {
            $.ajax({
                type: "POST",
                url: '/Cart/RemoveFromCart',
                data: { productId: productId },
                success: function (response) {
                    location.reload(); // Перезагружаем страницу, чтобы отобразить изменения
                },
                error: function () {
                    alert('Не удалось удалить товар из корзины.');
                }
            });
        } else {
            $.ajax({
                type: "POST",
                url: '/Cart/UpdateQuantity',
                contentType: "application/x-www-form-urlencoded; charset=utf-8",
                data: $.param(requestData),
                success: function (response) {
                    if (response.total !== undefined) {
                        // Обновляем количество и стоимость товара
                        $(`#quantity-${productId}`).text(updatedQuantity);
                        var newTotal = (price * updatedQuantity).toFixed(2);
                        $(`#total-${productId}`).text(newTotal + '€');

                        // Обновляем subtotal на странице
                        $('.subtotal-amount').text(`${response.subtotal.toFixed(2)}€`);
                        
                        updateSubtotal();
                        toastr.success('Количество товара обновлено.');
                    } else {
                        toastr.error('Ошибка при обновлении количества.');
                    }
                },
                error: function (xhr, status, error) {
                    console.error("Ошибка AJAX:", xhr.responseText);
                    toastr.error('Не удалось обновить количество товара.');
                }
            });
        }
    });

    // Обработчик для чекбоксов
    $('.item-extra input[type="checkbox"]').on('change', function () {
        var productId = $(this).closest('.book_cart').find('.btn-remove').data('product-id');
        var isWrapped = $(this).closest('.item-extra').find('input[type="checkbox"]').eq(0).is(':checked');
        var isStamped = $(this).closest('.item-extra').find('input[type="checkbox"]').eq(1).is(':checked');
        console.log("IMA here");
        // Отправка данных на сервер
        $.ajax({
            type: "POST",
            url: '/Cart/UpdateItemOptions',
            data: {
                productId: productId,
                isWrapped: isWrapped,
                isStamped: isStamped
            },
            success: function (response) {
                toastr.success('Опции обновлены.');
            },
            error: function () {
                toastr.error('Не удалось обновить опции.');
            }
        });
    });
});