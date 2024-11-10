const slider = document.querySelector('.slider');
const prevBtn = document.querySelector('.prev');
const nextBtn = document.querySelector('.next');
const slides = document.querySelectorAll('.slider img');

let currentSlide = 0;

// Показать текущий слайд с использованием transform
function showSlide(index) {
    const slideWidth = slides[index].clientWidth;
    slider.style.transform = `translateX(${-index * slideWidth}px)`;
}

// Назначаем обработчик клика для кнопки 'Previous'
prevBtn.addEventListener('click', () => {
    currentSlide = (currentSlide <= 0) ? slides.length - 1 : currentSlide - 1;
    showSlide(currentSlide);
});

// Назначаем обработчик клика для кнопки 'Next'
nextBtn.addEventListener('click', () => {
    currentSlide = (currentSlide >= slides.length - 1) ? 0 : currentSlide + 1;
    showSlide(currentSlide);
});

// Инициируем первый показ слайда
window.addEventListener('resize', () => showSlide(currentSlide));
showSlide(currentSlide);


function toggleMenu() {
    const nav = document.querySelector('.burger_info_nav');
    nav.classList.toggle('active');
}
