CREATE TABLE IF NOT EXISTS Reviews (
    ReviewID SERIAL PRIMARY KEY,           -- Уникальный идентификатор отзыва
    CustomerID INT REFERENCES Customers(CustomerID),  -- Ссылка на клиента
    BookID INT REFERENCES Books(BookID),    -- Ссылка на книгу
    Rating INT CHECK (Rating BETWEEN 1 AND 5),  -- Оценка книги (от 1 до 5)
    Comment TEXT,                          -- Комментарий покупателя
    ReviewDate DATE NOT NULL               -- Дата отзыва
);

CREATE TABLE IF NOT EXISTS OrderStatusHistory (
    OrderStatusID SERIAL PRIMARY KEY,      -- Уникальный идентификатор истории статуса заказа
    OrderID INT REFERENCES Orders(OrderID),  -- Ссылка на заказ
    Status VARCHAR(255) NOT NULL,          -- Статус заказа (например, "Ожидает оплаты", "Отправлен", "Завершен")
    ChangeDate DATE NOT NULL                -- Дата изменения статуса
);

CREATE TABLE IF NOT EXISTS PriceHistory (
    PriceHistoryID SERIAL PRIMARY KEY,      -- Уникальный идентификатор истории цен
    BookID INT REFERENCES Books(BookID),     -- Ссылка на книгу
    OldPrice DECIMAL(10, 2) NOT NULL,       -- Старая цена
    NewPrice DECIMAL(10, 2) NOT NULL,       -- Новая цена
    ChangeDate DATE NOT NULL                 -- Дата изменения цены
);

CREATE TABLE IF NOT EXISTS BookAuthors 
(
    BookAuthorID SERIAL PRIMARY KEY,      -- Уникальный идентификатор связи книги и автора
    BookID INT REFERENCES Books(BookID),   -- Ссылка на книгу
    AuthorID INT REFERENCES Authors(AuthorID)  -- Ссылка на автора
);


CREATE TABLE IF NOT EXISTS Orders (
    OrderID SERIAL PRIMARY KEY,        -- Уникальный идентификатор заказа
    CustomerID INT REFERENCES Customers(CustomerID),  -- Ссылка на клиента
    OrderDate DATE NOT NULL,           -- Дата заказа
    TotalAmount DECIMAL(10, 2) NOT NULL,  -- Общая сумма заказа
    Status VARCHAR(255) NOT NULL       -- Статус заказа
);

CREATE TABLE IF NOT EXISTS OrderDetails (
    OrderDetailID SERIAL PRIMARY KEY,  -- Уникальный идентификатор детали заказа
    OrderID INT REFERENCES Orders(OrderID),   -- Ссылка на заказ
    BookID INT REFERENCES Books(BookID),      -- Ссылка на книгу
    Quantity INT NOT NULL,                  -- Количество заказанных книг
    UnitPrice DECIMAL(10, 2) NOT NULL      -- Цена за одну книгу на момент заказа
);

CREATE TABLE IF NOT EXISTS Customers
(
	CustomerID SERIAL PRIMARY KEY,
	FirstName VARCHAR(255),
	LastName VARCHAR(255),
	Email VARCHAR(255) UNIQUE CHECK (Email <> ' ' AND Email <> ''),
	PhoneNumber VARCHAR(20),
	Address TEXT
);

CREATE TABLE IF NOT EXISTS Genres
(
	GenreID SERIAL PRIMARY KEY,
	GenresName VARCHAR(255) UNIQUE,
	Description TEXT
	
);

CREATE TABLE IF NOT EXISTS Publishers 
(
    PublisherID SERIAL PRIMARY KEY,   
	PublisherName VARCHAR(255) NOT NULL,       
	ContactInfo VARCHAR(255) 
);
CREATE TABLE IF NOT EXISTS Authors
(
	AuthorID SERIAL PRIMARY KEY,
    FirstName VARCHAR(255) NOT NULL,
    LastName VARCHAR(255) NOT NULL,
    Bio TEXT DEFAULT NULL
);

CREATE TABLE IF NOT EXISTS Books
(
	BookID SERIAL PRIMARY KEY,                    -- Уникальный идентификатор книги
    Title VARCHAR(255) NOT NULL,                  -- Название книги
    AuthorID INT REFERENCES Authors(AuthorID),    -- Ссылка на автора (внешний ключ)
    PublisherID INT REFERENCES Publishers(PublisherID), -- Ссылка на издателя (внешний ключ)
    GenreID INT REFERENCES Genres(GenreID),       -- Ссылка на жанр (внешний ключ)
    Price DECIMAL(10, 2) CHECK (Price > 0),                -- Цена книги
    ISBN VARCHAR(13) UNIQUE NOT NULL,             -- Международный стандартный книжный номер
    StockQuantity INT DEFAULT 0,                  -- Количество экземпляров на складе
    Description TEXT,                             -- Описание книги
    ImageLinks JSON    
);