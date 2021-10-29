// See https://aka.ms/new-console-template for more information

using Bogdanov.ConsoleWorkLib;

C.InfoH("Приложение предназначено для тестирования и демонстрации возможностей библиотеки Bogdanov.ConsoleWorkLib");
C.InfoH("Список примеров:");
C.InfoH("1 - выводит примеры отображения надписей разных цветов:");
string exampleNum = C.Interract("Выберите номер примера: ");
switch (exampleNum)
{
    case "1":
        DesignExamples.FontColors();
        break;
    default:
        break;
}
