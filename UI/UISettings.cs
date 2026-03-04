using Avalonia;

namespace PaintApp.UI;

public class UISettings
{
    public const int DeltaWindowCanvasWidth = 90;
    public const int DeltaWindowCanvasHeight = 80;
    public readonly string HelpClickText = 
    @"Рада приветствовать, дорогой пользователь!

Данная программа представляет собой инструмент для рисования.
Присутствует возможность как открытия файла для редактирования,
так и сохранения полученной картинки в формате Png.
По всем вопросам обращаться к разработчикам: sonya.lopuhova@yandex.ru

                            Успехов!";
    public Point? FirstLinePoint = null;
}