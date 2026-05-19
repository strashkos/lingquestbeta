using LinguaQuest.Models;

namespace LinguaQuest.Data;

public static class WordSeedData
{
    public static IReadOnlyList<Word> All => _all ??= BuildAll();
    private static List<Word>? _all;

    private static List<Word> BuildAll()
    {
        var list = new List<Word>();
        list.AddRange(EnglishA1());
        list.AddRange(EnglishA2());
        list.AddRange(EnglishB1());
        return list;
    }

    private static IEnumerable<Word> EnglishA1() => CreateSet(
        LearningLevel.A1,
        ("Привіт", "Hello", "Вітаються", "Hi", "Bye", "Thanks", "Please"),
        ("Дякую", "Thank you", "Подяка", "Sorry", "Hello", "Yes", "No"),
        ("Будь ласка", "Please", "Прохання", "Thanks", "Sorry", "Hello", "Yes"),
        ("Так", "Yes", "Згода", "No", "Maybe", "Sure", "Never"),
        ("Ні", "No", "Відмова", "Yes", "Not", "None", "Never"),
        ("Доброго ранку", "Good morning", "До обіду", "Good night", "Hello", "Goodbye", "Thanks"),
        ("Добраніч", "Good night", "Перед сном", "Good morning", "Hello", "Bye", "Sleep"),
        ("Вода", "Water", "П'ють", "Fire", "Wine", "Juice", "Tea"),
        ("Їжа", "Food", "Їдять", "Drink", "Book", "Home", "Work"),
        ("Хліб", "Bread", "Випічка", "Butter", "Milk", "Cheese", "Soup"),
        ("Молоко", "Milk", "Білий напій", "Water", "Juice", "Tea", "Coffee"),
        ("Дім", "House", "Живуть", "Home", "Room", "Door", "Street"),
        ("Кімната", "Room", "У будинку", "House", "Door", "Window", "Bed"),
        ("Сім'я", "Family", "Рідні", "Friend", "Work", "School", "Team"),
        ("Мама", "Mother", "Батьки", "Father", "Sister", "Brother", "Family"),
        ("Тато", "Father", "Батьки", "Mother", "Son", "Daughter", "Uncle"),
        ("Друг", "Friend", "Товариш", "Enemy", "Guest", "Host", "Team"),
        ("Школа", "School", "Вчаться", "Work", "Park", "Shop", "Bank"),
        ("Вчитель", "Teacher", "У класі", "Student", "School", "Book", "Desk"),
        ("Учень", "Student", "Навчається", "Teacher", "School", "Class", "Book"),
        ("Робота", "Work", "Працюють", "Rest", "Game", "Sleep", "Walk"),
        ("Час", "Time", "Годинник", "Day", "Night", "Week", "Year"),
        ("День", "Day", "Сонце", "Night", "Week", "Hour", "Month"),
        ("Ніч", "Night", "Темно", "Day", "Light", "Dark", "Moon"),
        ("Сьогодні", "Today", "Зараз", "Tomorrow", "Yesterday", "Now", "Always")
    );

    private static IEnumerable<Word> EnglishA2() => CreateSet(
        LearningLevel.A2,
        ("Подорож", "Journey", "Маршрут", "Trip", "Travel", "Tour", "Visit"),
        ("Аеропорт", "Airport", "Літак", "Station", "Port", "Hotel", "Ticket"),
        ("Готель", "Hotel", "Номер", "Hostel", "House", "Camp", "Room"),
        ("Квиток", "Ticket", "Каса", "Pass", "Card", "Bill", "Receipt"),
        ("Паспорт", "Passport", "Документ", "Ticket", "Visa", "ID", "Card"),
        ("Валіза", "Suitcase", "Багаж", "Bag", "Box", "Pack", "Case"),
        ("Здоров'я", "Health", "Лікар", "Illness", "Pain", "Care", "Cure"),
        ("Лікар", "Doctor", "Клініка", "Nurse", "Patient", "Drug", "Cure"),
        ("Аптека", "Pharmacy", "Ліки", "Hospital", "Clinic", "Drug", "Pill"),
        ("Погода", "Weather", "Дощ чи сонце", "Rain", "Snow", "Wind", "Cloud"),
        ("Дощ", "Rain", "Парасолька", "Snow", "Sun", "Storm", "Fog"),
        ("Сонце", "Sun", "Світло", "Moon", "Star", "Cloud", "Sky"),
        ("Покупка", "Purchase", "Магазин", "Sale", "Price", "Cost", "Pay"),
        ("Ціна", "Price", "Коштує", "Cost", "Sale", "Bill", "Cash"),
        ("Знижка", "Discount", "Дешевше", "Sale", "Price", "Offer", "Deal"),
        ("Ресторан", "Restaurant", "Меню", "Cafe", "Bar", "Kitchen", "Chef"),
        ("Меню", "Menu", "Страва", "Bill", "Order", "Plate", "Dish"),
        ("Офіціант", "Waiter", "Замовлення", "Chef", "Cook", "Guest", "Tip"),
        ("Спорт", "Sport", "Тренування", "Game", "Team", "Win", "Lose"),
        ("Команда", "Team", "Разом", "Group", "Club", "Player", "Coach"),
        ("Музика", "Music", "Слухають", "Song", "Dance", "Band", "Note"),
        ("Фільм", "Movie", "Кіно", "Show", "Actor", "Scene", "Play"),
        ("Новина", "News", "ЗМІ", "Paper", "Report", "Story", "Fact"),
        ("Інтернет", "Internet", "Онлайн", "Web", "Site", "Email", "Link"),
        ("Електронна пошта", "Email", "Надіслати", "Letter", "Message", "Chat", "Call")
    );

    private static IEnumerable<Word> EnglishB1() => CreateSet(
        LearningLevel.B1,
        ("Досвід", "Experience", "Набувають", "Skill", "Habit", "Trial", "Test"),
        ("Успіх", "Success", "Досягнення", "Failure", "Goal", "Result", "Win"),
        ("Помилка", "Mistake", "Виправляють", "Error", "Fault", "Bug", "Fix"),
        ("Рішення", "Decision", "Обирають", "Choice", "Plan", "Idea", "Vote"),
        ("Можливість", "Opportunity", "Шанс", "Risk", "Luck", "Fate", "Hope"),
        ("Відповідальність", "Responsibility", "Обов'язок", "Duty", "Role", "Task", "Job"),
        ("Дедлайн", "Deadline", "Термін", "Delay", "Schedule", "Plan", "Date"),
        ("Пріоритет", "Priority", "Важливе", "Task", "Goal", "Plan", "Order"),
        ("Культура", "Culture", "Традиції", "Art", "Custom", "Rule", "Norm"),
        ("Суспільство", "Society", "Люди", "Group", "Nation", "Class", "Crowd"),
        ("Економіка", "Economy", "Ринок", "Trade", "Business", "Market", "Bank"),
        ("Інфляція", "Inflation", "Ціни", "Tax", "Debt", "Loan", "Rate"),
        ("Політика", "Politics", "Влада", "Law", "State", "Party", "Vote"),
        ("Демократія", "Democracy", "Голос", "Law", "Right", "Freedom", "Vote"),
        ("Навколишнє середовище", "Environment", "Екологія", "Nature", "Climate", "Green", "Pollution"),
        ("Технологія", "Technology", "Прогрес", "Science", "Device", "Tool", "Code"),
        ("Штучний інтелект", "Artificial intelligence", "AI", "Robot", "Data", "Model", "Code"),
        ("Аргумент", "Argument", "Дискусія", "Reason", "Proof", "Claim", "Fact"),
        ("Угода", "Agreement", "Договір", "Deal", "Contract", "Terms", "Sign"),
        ("Конфлікт", "Conflict", "Суперечка", "Fight", "Peace", "War", "Clash"),
        ("Компроміс", "Compromise", "Згода", "Deal", "Talk", "Peace", "Win"),
        ("Ставлення", "Attitude", "Погляд", "View", "Mood", "Mind", "Feeling"),
        ("Поведінка", "Behavior", "Вчинки", "Action", "Habit", "Rule", "Norm"),
        ("Розвиток", "Development", "Зростання", "Growth", "Change", "Progress", "Stage"),
        ("Інновація", "Innovation", "Нове", "Idea", "Change", "Tech", "Growth")
    );

    private static IEnumerable<Word> CreateSet(
        LearningLevel level,
        params (string Uk, string Target, string Hint, string W1, string W2, string W3, string W4)[] rows)
    {
        foreach (var row in rows)
        {
            var options = new List<string> { row.Target, row.W1, row.W2, row.W3 };
            ShuffleOptions(options);

            yield return new Word
            {
                SourceLanguage = (int)LearningLanguage.Ukrainian,
                TargetLanguage = (int)LearningLanguage.English,
                Level = (int)level,
                SourceText = row.Uk,
                TargetText = row.Target,
                Hint = row.Hint,
                Category = level.ToString(),
                Options = options
            };
        }
    }

    private static void ShuffleOptions(List<string> options)
    {
        for (int i = options.Count - 1; i > 0; i--)
        {
            int j = Random.Shared.Next(i + 1);
            (options[i], options[j]) = (options[j], options[i]);
        }
    }
}
