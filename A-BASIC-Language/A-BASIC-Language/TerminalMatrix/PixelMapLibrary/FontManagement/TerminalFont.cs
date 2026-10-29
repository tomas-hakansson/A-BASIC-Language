#nullable enable
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace A_BASIC_Language.TerminalMatrix.PixelMapLibrary.FontManagement;

public class TerminalFont : IReadOnlyDictionary<char, PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix>
{
    private readonly Dictionary<char, PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix> _data;

    public TerminalFont()
    {
        _data = new Dictionary<char, PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix>
        {
            {
                ' ', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",,,,,,,," +
                    ",,,,,,,," +
                    ",,,,,,,," +
                    ",,,,,,,," +
                    ",,,,,,,," +
                    ",,,,,,,," +
                    ",,,,,,,," +
                    ",,,,,,,,"
                )
            },
            {
                '@', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",,****,," +
                    ",*,,,,*," +
                    "*,,**,,*" +
                    "*,*,,*,*" +
                    "*,,*,*,*" +
                    ",*,,,,*," +
                    ",,,**,,," +
                    ",,,,,,,,"
                )
            },
            {
                '(', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",,,,**,," +
                    ",,,**,,," +
                    ",,**,,,," +
                    ",,**,,,," +
                    ",,**,,,," +
                    ",,**,,,," +
                    ",,,**,,," +
                    ",,,,**,,"
                )
            },
            {
                ')', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",,**,,,," +
                    ",,,**,,," +
                    ",,,,**,," +
                    ",,,,**,," +
                    ",,,,**,," +
                    ",,,,**,," +
                    ",,,**,,," +
                    ",,**,,,,"
                )
            },
            {
                '<', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",,,,,,,," +
                    ",,,,**,," +
                    ",,,**,,," +
                    ",,**,,,," +
                    ",,**,,,," +
                    ",,,**,,," +
                    ",,,,**,," +
                    ",,,,,,,,"
                )
            },
            {
                '>', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",,,,,,,," +
                    ",,**,,,," +
                    ",,,**,,," +
                    ",,,,**,," +
                    ",,,,**,," +
                    ",,,**,,," +
                    ",,**,,,," +
                    ",,,,,,,,"
                )
            },
            {
                '#', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",,**,**," +
                    ",*******" +
                    ",,**,**," +
                    ",,**,**," +
                    ",,**,**," +
                    ",*******" +
                    ",,**,**," +
                    ",,,,,,,,"
                )
            },
            {
                '$', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",,,**,,," +
                    ",,****,," +
                    ",**,,,,," +
                    ",,****,," +
                    ",,,,,**," +
                    ",,****,," +
                    ",,,**,,," +
                    ",,,,,,,,"
                )
            },
            {
                '&', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",,****,," +
                    ",**,,**," +
                    ",,****,," +
                    ",****,,," +
                    ",**,,***" +
                    ",**,,**," +
                    ",,******" +
                    ",,,,,,,,"
                )
            },
            {
                ':', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",,,,,,,," +
                    ",,,,,,,," +
                    ",,,**,,," +
                    ",,,**,,," +
                    ",,,,,,,," +
                    ",,,**,,," +
                    ",,,**,,," +
                    ",,,,,,,,"
                )
            },
            {
                '+', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",,,,,,,," +
                    ",,,**,,," +
                    ",,,**,,," +
                    ",******," +
                    ",,,**,,," +
                    ",,,**,,," +
                    ",,,,,,,," +
                    ",,,,,,,,"
                )
            },
            {
                '*', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",,,,,,,," +
                    ",**,,**," +
                    ",,****,," +
                    "********" +
                    ",,****,," +
                    ",**,,**," +
                    ",,,,,,,," +
                    ",,,,,,,,"
                )
            },
            {
                '/', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",,,,,,,," +
                    ",,,,**,," +
                    ",,,,**,," +
                    ",,,**,,," +
                    ",,,**,,," +
                    ",,**,,,," +
                    ",,**,,,," +
                    ",,,,,,,,"
                )
            },
            {
                '\\', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",,,,,,,," +
                    ",,**,,,," +
                    ",,**,,,," +
                    ",,,**,,," +
                    ",,,**,,," +
                    ",,,,**,," +
                    ",,,,**,," +
                    ",,,,,,,,"
                )
            },
            {
                '%', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",,,,,,,," +
                    "**,,**,," +
                    "**,,**,," +
                    ",,,**,,," +
                    ",,,**,,," +
                    ",,**,,**" +
                    ",,**,,**" +
                    ",,,,,,,,"
                )
            },
            {
                ',', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",,,,,,,," +
                    ",,,,,,,," +
                    ",,,,,,,," +
                    ",,,,,,,," +
                    ",,,,,,,," +
                    ",,,**,,," +
                    ",,,**,,," +
                    ",,**,,,,"
                )
            },
            {
                ';', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",,,,,,,," +
                    ",,,,,,,," +
                    ",,,,,,,," +
                    ",,,**,,," +
                    ",,,,,,,," +
                    ",,,**,,," +
                    ",,,**,,," +
                    ",,**,,,,"
                )
            },
            {
                '.', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",,,,,,,," +
                    ",,,,,,,," +
                    ",,,,,,,," +
                    ",,,,,,,," +
                    ",,,,,,,," +
                    ",,,**,,," +
                    ",,,**,,," +
                    ",,,,,,,,"
                )
            },
            {
                '"', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",**,,**," +
                    ",**,,**," +
                    ",,,,,,,," +
                    ",,,,,,,," +
                    ",,,,,,,," +
                    ",,,,,,,," +
                    ",,,,,,,," +
                    ",,,,,,,,"
                )
            },
            {
                '\'', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",,,**,,," +
                    ",,,**,,," +
                    ",,,,,,,," +
                    ",,,,,,,," +
                    ",,,,,,,," +
                    ",,,,,,,," +
                    ",,,,,,,," +
                    ",,,,,,,,"
                )
            },
            {
                '0', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",,****,," +
                    ",**,,**," +
                    ",**,***," +
                    ",******," +
                    ",***,**," +
                    ",**,,**," +
                    ",,****,," +
                    ",,,,,,,,"
                )
            },
            {
                '!', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",,,**,,," +
                    ",,,**,,," +
                    ",,,**,,," +
                    ",,,**,,," +
                    ",,,**,,," +
                    ",,,,,,,," +
                    ",,,**,,," +
                    ",,,,,,,,"
                )
            },
            {
                '?', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",,****,," +
                    ",**,,**," +
                    ",,,,,**," +
                    ",,,,**,," +
                    ",,,**,,," +
                    ",,,,,,,," +
                    ",,,**,,," +
                    ",,,,,,,,"
                )
            },
            {
                '1', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",,,**,,," +
                    ",,***,,," +
                    ",****,,," +
                    ",,,**,,," +
                    ",,,**,,," +
                    ",,,**,,," +
                    ",******," +
                    ",,,,,,,,"
                )
            },
            {
                '2', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",,****,," +
                    ",**,,**," +
                    ",,,, **," +
                    ",,, **,," +
                    ",, **,,," +
                    ", **,,,," +
                    ",******," +
                    ",,,,,,,,"
                )
            },
            {
                '3', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",,****,," +
                    ",**,,**," +
                    ",,,,,**," +
                    ",,,,**,," +
                    ",,,,,**," +
                    ",**,,**," +
                    ",,****,," +
                    ",,,,,,,,"
                )
            },
            {
                '4', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",,,,***," +
                    ",,,****," +
                    ",,**,**," +
                    ",**,,**," +
                    ",******," +
                    ",,,,,**," +
                    ",,,,,**," +
                    ",,,,,,,,"
                )
            },
            {
                '5', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",******," +
                    ",**,,,,," +
                    ",**,,,,," +
                    ",*****,," +
                    ",,,,,**," +
                    ",**,,**," +
                    ",,****,," +
                    ",,,,,,,,"
                )
            },
            {
                '6', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",,****,," +
                    ",**,,**," +
                    ",**,,,,," +
                    ",*****,," +
                    ",**,,**," +
                    ",**,,**," +
                    ",,****,," +
                    ",,,,,,,,"
                )
            },
            {
                '7', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",******," +
                    ",,,,,**," +
                    ",,,,,**," +
                    ",,,,**,," +
                    ",,,**,,," +
                    ",,,**,,," +
                    ",,,**,,," +
                    ",,,,,,,,"
                )
            },
            {
                '8', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",,****,," +
                    ",**,,**," +
                    ",**,,**," +
                    ",,****,," +
                    ",**,,**," +
                    ",**,,**," +
                    ",,****,," +
                    ",,,,,,,,"
                )
            },
            {
                '9', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",,****,," +
                    ",**,,**," +
                    ",**,,**," +
                    ",,*****," +
                    ",,,,,**," +
                    ",,,,,**," +
                    ",,****,," +
                    ",,,,,,,,"
                )
            },
            {
                'A', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",,,**,,," +
                    ",,****,," +
                    ",**,,**," +
                    ",**,,**," +
                    ",******," +
                    ",**,,**," +
                    ",**,,**," +
                    ",,,,,,,,"
                )
            },
            {
                'a', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",,,,,,,," +
                    ",,,,,,,," +
                    ",,****,," +
                    ",,,,,**," +
                    ",,*****," +
                    ",**,,**," +
                    ",,*****," +
                    ",,,,,,,,"
                )
            },
            {
                'B', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",*****,," +
                    ",**,,**," +
                    ",**,,**," +
                    ",*****,," +
                    ",**,,**," +
                    ",**,,**," +
                    ",*****,," +
                    ",,,,,,,,"
                )
            },
            {
                'b', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",**,,,,," +
                    ",**,,,,," +
                    ",*****,," +
                    ",**,,**," +
                    ",**,,**," +
                    ",**,,**," +
                    ",*****,," +
                    ",,,,,,,,"
                )
            },
            {
                'C', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",,****,," +
                    ",**,,**," +
                    ",**,,,,," +
                    ",**,,,,," +
                    ",**,,,,," +
                    ",**,,**," +
                    ",,****,," +
                    ",,,,,,,,"
                )
            },
            {
                'c', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",,,,,,,," +
                    ",,,,,,,," +
                    ",,****,," +
                    ",**,,**," +
                    ",**,,,,," +
                    ",**,,**," +
                    ",,****,," +
                    ",,,,,,,,"
                )
            },
            {
                'D', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",****,,," +
                    ",**,**,," +
                    ",**,,**," +
                    ",**,,**," +
                    ",**,,**," +
                    ",**,**,," +
                    ",****,,," +
                    ",,,,,,,,"
                )
            },
            {
                'd', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",,,,,**," +
                    ",,,,,**," +
                    ",,*****," +
                    ",**,,**," +
                    ",**,,**," +
                    ",**,'**," +
                    ",,*****," +
                    ",,,,,,,,"
                )
            },
            {
                'E', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",******," +
                    ",**,,,,," +
                    ",**,,,,," +
                    ",****,,," +
                    ",**,,,,," +
                    ",**,,,,," +
                    ",******," +
                    ",,,,,,,,"
                )
            },
            {
                'e', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",,,,,,,," +
                    ",,,,,,,," +
                    ",,****,," +
                    ",**,,**," +
                    ",******," +
                    ",**,,,,," +
                    ",,****,," +
                    ",,,,,,,,"
                )
            },
            {
                'F', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",******," +
                    ",**,,,,," +
                    ",**,,,,," +
                    ",****,,," +
                    ",**,,,,," +
                    ",**,,,,," +
                    ",**,,,,," +
                    ",,,,,,,,"
                )
            },
            {
                'f', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",,,****," +
                    ",,**,,,," +
                    ",,**,,,," +
                    ",****,,," +
                    ",,**,,,," +
                    ",,**,,,," +
                    ",,**,,,," +
                    ",,,,,,,,"
                )
            },
            {
                'G', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",,****,," +
                    ",**,,**," +
                    ",**,,,,," +
                    ",**,***," +
                    ",**,,**," +
                    ",**,,**," +
                    ",,****,," +
                    ",,,,,,,,"
                )
            },
            {
                'g', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",,,,,,,," +
                    ",,,,,,,," +
                    ",,*****," +
                    ",**,,**," +
                    ",**,,**," +
                    ",,*****," +
                    ",,,,,**," +
                    ",,****,,"
                )
            },
            {
                'H', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",**,,**," +
                    ",**,,**," +
                    ",**,,**," +
                    ",******," +
                    ",**,,**," +
                    ",**,,**," +
                    ",**,,**," +
                    ",,,,,,,,"
                )
            },
            {
                'h', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",**,,,,," +
                    ",**,,,,," +
                    ",*****,," +
                    ",**,,**," +
                    ",**,,**," +
                    ",**,,**," +
                    ",**,,**," +
                    ",,,,,,,,"
                )
            },
            {
                'I', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",,****,," +
                    ",,,**,,," +
                    ",,,**,,," +
                    ",,,**,,," +
                    ",,,**,,," +
                    ",,,**,,," +
                    ",,****,," +
                    ",,,,,,,,"
                )
            },
            {
                'i', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",,,**,,," +
                    ",,,,,,,," +
                    ",,,**,,," +
                    ",,,**,,," +
                    ",,,**,,," +
                    ",,,**,,," +
                    ",,,**,,," +
                    ",,,,,,,,"
                )
            },
            {
                'J', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",,,****," +
                    ",,,,,**," +
                    ",,,,,**," +
                    ",,,,,**," +
                    ",,,,,**," +
                    ",**,,**," +
                    ",,****,," +
                    ",,,,,,,,"
                )
            },
            {
                'j', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",,,,,**," +
                    ",,,,,,,," +
                    ",,,,,**," +
                    ",,,,,**," +
                    ",,,,,**," +
                    ",,,,,**," +
                    ",**,,**," +
                    ",,****,,"
                )
            },
            {
                'K', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",**,,**," +
                    ",**,,**," +
                    ",**,**,," +
                    ",****,,," +
                    ",**,**,," +
                    ",**,,**," +
                    ",**,,**," +
                    ",,,,,,,,"
                )
            },
            {
                'k', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",**,,,,," +
                    ",**,,,,," +
                    ",**,,**," +
                    ",**,**,," +
                    ",****,,," +
                    ",**,**,," +
                    ",**,,**," +
                    ",,,,,,,,"
                )
            },
            {
                'L', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",**,,,,," +
                    ",**,,,,," +
                    ",**,,,,," +
                    ",**,,,,," +
                    ",**,,,,," +
                    ",**,,,,," +
                    ",******," +
                    ",,,,,,,,"
                )
            },
            {
                'l', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",,***,,," +
                    ",,,**,,," +
                    ",,,**,,," +
                    ",,,**,,," +
                    ",,,**,,," +
                    ",,,**,,," +
                    ",,****,," +
                    ",,,,,,,,"
                )
            },
            {
                'M', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",**,,,**" +
                    ",***,***" +
                    ",*******" +
                    ",**,*,**" +
                    ",**,,,**" +
                    ",**,,,**" +
                    ",**,,,**" +
                    ",,,,,,,,"
                )
            },
            {
                'm', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",,,,,,,," +
                    ",,,,,,,," +
                    ",***,**," +
                    ",*******" +
                    ",**,*,**" +
                    ",**,,,**" +
                    ",**,,,**" +
                    ",,,,,,,,"
                )
            },
            {
                'N', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",**,,**," +
                    ",***,**," +
                    ",******," +
                    ",**,***," +
                    ",**,,**," +
                    ",**,,**," +
                    ",**,,**," +
                    ",,,,,,,,"
                )
            },
            {
                'n', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",,,,,,,," +
                    ",,,,,,,," +
                    ",******," +
                    ",**,,,**" +
                    ",**,,,**" +
                    ",**,,,**" +
                    ",**,,,**" +
                    ",,,,,,,,"
                )
            },
            {
                'O', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",,****,," +
                    ",**,,**," +
                    ",**,,**," +
                    ",**,,**," +
                    ",**,,**," +
                    ",**,,**," +
                    ",,****,," +
                    ",,,,,,,,"
                )
            },
            {
                'o', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",,,,,,,," +
                    ",,,,,,,," +
                    ",,****,," +
                    ",**,,**," +
                    ",**,,**," +
                    ",**,,**," +
                    ",,****,," +
                    ",,,,,,,,"
                )
            },
            {
                'P', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",*****,," +
                    ",**,,**," +
                    ",**,,**," +
                    ",*****,," +
                    ",**,,,,," +
                    ",**,,,,," +
                    ",**,,,,," +
                    ",,,,,,,,"
                )
            },
            {
                'p', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",,,,,,,," +
                    ",,,,,,,," +
                    ",*****,," +
                    ",**,,**," +
                    ",**,,**," +
                    ",*****,," +
                    ",**,,,,," +
                    ",**,,,,,"
                )
            },
            {
                'Q', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",,****,," +
                    ",**,,**," +
                    ",**,,**," +
                    ",**,,**," +
                    ",**,,**," +
                    ",**,,**," +
                    ",,****,," +
                    ",,,,,**,"
                )
            },
            {
                'q', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",,,,,,,," +
                    ",,,,,,,," +
                    ",,*****," +
                    ",**,,**," +
                    ",**,,**," +
                    ",,*****," +
                    ",,,,,**," +
                    ",,,,,**,"
                )
            },
            {
                'R', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",*****,," +
                    ",**,,**," +
                    ",**,,**," +
                    ",*****,," +
                    ",**,,**," +
                    ",**,,**," +
                    ",**,,**," +
                    ",,,,,,,,"
                )
            },
            {
                'r', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",,,,,,,," +
                    ",,,,,,,," +
                    ",*****,," +
                    ",**,,**," +
                    ",**,,,,," +
                    ",**,,,,," +
                    ",**,,,,," +
                    ",,,,,,,,"
                )
            },
            {
                'S', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",,****,," +
                    ",**,,**," +
                    ",**,,,,," +
                    ",,****,," +
                    ",,,,,**," +
                    ",**,,**," +
                    ",,****,," +
                    ",,,,,,,,"
                )
            },
            {
                's', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",,,,,,,," +
                    ",,,,,,,," +
                    ",,****,," +
                    ",**,,,,," +
                    ",,****,," +
                    ",,,,,**," +
                    ",,****,," +
                    ",,,,,,,,"
                )
            },
            {
                'T', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",******," +
                    ",,,**,,," +
                    ",,,**,,," +
                    ",,,**,,," +
                    ",,,**,,," +
                    ",,,**,,," +
                    ",,,**,,," +
                    ",,,,,,,,"
                )
            },
            {
                't', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",,**,,,," +
                    ",,**,,,," +
                    ",*****,," +
                    ",,**,,,," +
                    ",,**,,,," +
                    ",,**,,,," +
                    ",,,****," +
                    ",,,,,,,,,"
                )
            },
            {
                'U', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",**,,**," +
                    ",**,,**," +
                    ",**,,**," +
                    ",**,,**," +
                    ",**,,**," +
                    ",**,,**," +
                    ",,****,," +
                    ",,,,,,,,"
                )
            },
            {
                'u', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",,,,,,,," +
                    ",,,,,,,," +
                    ",**,,**," +
                    ",**,,**," +
                    ",**,,**," +
                    ",**,,**," +
                    ",,****,," +
                    ",,,,,,,,"
                )
            },
            {
                'V', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",**,,**," +
                    ",**,,**," +
                    ",**,,**," +
                    ",**,,**," +
                    ",**,,**," +
                    ",,****,," +
                    ",,,**,,," +
                    ",,,,,,,,"
                )
            },
            {
                'v', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",,,,,,,," +
                    ",,,,,,,," +
                    ",**,,**," +
                    ",**,,**," +
                    ",**,,**," +
                    ",,****,," +
                    ",,,**,,," +
                    ",,,,,,,,"
                )
            },
            {
                'W', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",**,,,**" +
                    ",**,,,**" +
                    ",**,,,**" +
                    ",**,*,**" +
                    ",*******" +
                    ",***,***" +
                    ",**,,,**" +
                    ",,,,,,,,"
                )
            },
            {
                'w', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",,,,,,,," +
                    ",,,,,,,," +
                    ",**,,,**" +
                    ",**,*,**" +
                    ",*******" +
                    ",***,***" +
                    ",**,,,**" +
                    ",,,,,,,,,"
                )
            },
            {
                'X', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",**,,**," +
                    ",**,,**," +
                    ",,****,," +
                    ",,,**,,," +
                    ",,****,," +
                    ",**,,**," +
                    ",**,,**," +
                    ",,,,,,,,"
                )
            },
            {
                'x', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",,,,,,,," +
                    ",,,,,,,," +
                    ",**,,**," +
                    ",,****,," +
                    ",,,**,,," +
                    ",,****,," +
                    ",**,,**," +
                    ",,,,,,,,"
                )
            },
            {
                'Y', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",**,,**," +
                    ",**,,**," +
                    ",**,,**," +
                    ",,****,," +
                    ",,,**,,," +
                    ",,,**,,," +
                    ",,,**,,," +
                    ",,,,,,,,"
                )
            },
            {
                'y', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",,,,,,,," +
                    ",,,,,,,," +
                    ",**,,**," +
                    ",**,,**," +
                    ",**,,**," +
                    ",,*****," +
                    ",,,,,**," +
                    ",,****,,"
                )
            },
            {
                'Z', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",******," +
                    ",,,,,**," +
                    ",,,,**,," +
                    ",,'**,,," +
                    ",,**,,,," +
                    ",**,,,,," +
                    ",******," +
                    ",,,,,,,,"
                )
            },
            {
                'z', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",,,,,,,," +
                    ",,,,,,,," +
                    ",******," +
                    ",,,,**,," +
                    ",,,**,,," +
                    ",,**,,,," +
                    ",******," +
                    ",,,,,,,,"
                )
            },
            {
                '-', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",,,,,,,," +
                    ",,,,,,,," +
                    ",,,,,,,," +
                    ",******," +
                    ",,,,,,,," +
                    ",,,,,,,," +
                    ",,,,,,,," +
                    ",,,,,,,,"
                )
            },
            {
                '=', new PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix(
                    ",,,,,,,," +
                    ",,,,,,,," +
                    ",******," +
                    ",,,,,,,," +
                    ",******," +
                    ",,,,,,,," +
                    ",,,,,,,," +
                    ",,,,,,,,"
                )
            }
        };
    }

    public IEnumerator<KeyValuePair<char, PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix>> GetEnumerator() =>
        _data.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() =>
        _data.GetEnumerator();

    public int Count =>
        _data.Count;

    public bool ContainsKey(char key) =>
        _data.ContainsKey(key);

#pragma warning disable CS8601 // Possible null reference assignment.
    public bool TryGetValue(char key, out PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix value) =>
        _data.TryGetValue(key, out value);
#pragma warning restore CS8601 // Possible null reference assignment.

    public PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix this[int index] =>
        _data.ElementAt(index).Value;

    public PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix this[char key] =>
        _data[key];

    public IEnumerable<char> Keys =>
        _data.Keys;

    public IEnumerable<PixelmapLibraryNetFramework.FontManagement.CharacterPixelMatrix> Values =>
        _data.Values;
}