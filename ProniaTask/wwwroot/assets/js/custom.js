const cartItemHolder = document.querySelector(".cart-item-holder");
const addToCartButtons = document.querySelectorAll(".add-to-cart");


addToCartButtons.forEach(button =>
    button.addEventListener("click", ev => {
        ev.preventDefault();

        console.log("ok");
        const href = ev.target.getAttribute("href");

        fetch(href).then(res => res.text()).then(data => {
            cartItemHolder.innerHTML = data;
        })
    })
)
