using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemMessages
{
    public string fillOutAllItems, gameSaved, goBackToGraffitiArea, yourID, IDNotFound, pressToTalk, enterConversation, eraseOrReset, closeConversation, didntMeanToSayThis, notEnoughSoap, insertYourNameFirst,
    iShouldAnswer, youSavedAStudent, finishedGraffiti, finishedDialogues, youCanMakeNewFriends;
    public string hat, hair, chest, glasses, lenses, shoes, eyes, pants, body, help, editorConfirm, editorCreate, editorDone, language, controls, graphics, save, load, exit, imStuck, bindings, pressStart, answerThePhone, openDoor, important, importantInstructions, instructionsT, pressFToRide, WASDToMove, tutorialD0, tutorialD1, tutorialD1Tap, tutorialD2, tutorialD3, tutorialG0, tutorialG1, tutorialG1Tap, tutorialG2,
        touchIcon, loginWrong;
}

public class ML : MonoBehaviour
{
    public enum Lang { en, it, fr }
    public enum Mess { fillOutAllItems, gameSaved, goBackToGraffitiArea, yourID, IDNotFound, pressToTalk, enterConversation, eraseOrReset, closeConversation, didntMeanToSayThis, notEnoughSoap, insertYourNameFirst,
    iShouldAnswer, youSavedAStudent}
    public enum Str { hat, hair, chest, glasses, lenses, shoes, eyes, pants, body, editorConfirm, editorCreate, editorDone, language, controls, graphics, save, load, exit, bindings, pressStart, answerThePhone, openDoor, important, importantInstructions, instructionsT, pressFToRide, WASDToMove, tutorialD0, tutorialD1, tutorialD1Tap, tutorialD2, tutorialD3, tutorialG0, tutorialG1, tutorialG1Tap, tutorialG2 }
    public static SystemMessages systemMessages;


    //public static string LocalizedStr(Player agent, Str str)
    //{
    //    Lang lang = Player.language;
    //    if (lang == Lang.eng)
    //    {
    //        switch (str)
    //        {
    //            case Str.hat:
    //                return "hat";
    //            case Str.hair:
    //                return "hair";
    //            case Str.chest:
    //                return "chest";
    //            case Str.glasses:
    //                return "glasses";
    //            case Str.lenses:
    //                return "lenses";
    //            case Str.shoes:
    //                return "shoes";
    //            case Str.eyes:
    //                return "eyes";
    //            case Str.pants:
    //                return "pants";
    //            case Str.body:
    //                return "body";
    //            case Str.editorConfirm:
    //                return "Confirm!";
    //            case Str.editorCreate:
    //                return "Create your character!";
    //            case Str.editorDone:
    //                return "Done, let me see!";
    //            case Str.controls:
    //                return "Controls";
    //            case Str.bindings:
    //                return "Move: WASD/Arrows \n Jump / Glider: SPACEBAR \n Interact: E / LEFT MOUSE BUTTON \n Camera zoom: MOUSE WHEEL \n Map: M \n \n To annotate the graffiti, simply press E near one and click and drag the sponge with your mouse. \n \n To annotate the dialogues, click on the words inside the blue cloud and insert new text (or no text) using the input field at the bottom of the screen.";
    //            case Str.instructionsT:
    //                return "Instructions";
    //            case Str.language:
    //                return "Language";
    //            case Str.important:
    //                return "IMPORTANT:";
    //            case Str.importantInstructions:
    //                return "If you are not sure a sentence is okay, you can leave it as it is and press ENTER or E. If you do not know how to correct the sentence even if it is wrong, you can erase just one word, or, if you prefer, the whole sentence.";
    //            case Str.save:
    //                return "Save";
    //            case Str.load:
    //                return "Load";
    //            case Str.exit:
    //                return "Exit";
    //            case Str.graphics:
    //                return "Graphics";
    //            case Str.pressStart:
    //                return "PRESS ENTER";
    //            case Str.answerThePhone:
    //                return "ANSWER";
    //            case Str.openDoor:
    //                return "OPEN";
    //            case Str.pressFToRide:
    //                return "Press F to ride!";
    //            case Str.WASDToMove:
    //                return "MOVE";
    //            case Str.tutorialD0:
    //                return "Press E";
    //            case Str.tutorialD1:
    //                return "Click on a word";
    //            case Str.tutorialD2:
    //                return "Enter text";
    //            case Str.tutorialD3:
    //                return "The sentence changes";
    //            case Str.tutorialG0:
    //                return "Press E";
    //            case Str.tutorialG1:
    //                return "Click anywhere";
    //            case Str.tutorialG2:
    //                return "Drag";
    //        }
    //    }
    //    else if (lang == Lang.ita)
    //    {
    //        switch (str)
    //        {
    //            case Str.hat:
    //                return "cappello";
    //            case Str.hair:
    //                return "capelli";
    //            case Str.chest:
    //                return "busto";
    //            case Str.glasses:
    //                return "occhiali";
    //            case Str.lenses:
    //                return "lenti";
    //            case Str.shoes:
    //                return "scarpe";
    //            case Str.eyes:
    //                return "occhi";
    //            case Str.pants:
    //                return "pantaloni";
    //            case Str.body:
    //                return "corpo";
    //            case Str.editorConfirm:
    //                return "Conferma!";
    //            case Str.editorCreate:
    //                return "Crea il tuo personaggio!";
    //            case Str.editorDone:
    //                return "Fatto, fammi vedere!";
    //            case Str.controls:
    //                return "Controlli";
    //            case Str.bindings:
    //                return "Muovi: WASD/Frecce \n Salta / Deltaplano: BARRA SPAZIATRICE \n Interagisci: E / MOUSE SX \n Zoom: ROTELLINA \n Mappa: M \n \n Per annotare i murales, premi E quando sei nelle vicinanze e clicca e trascina la spugna con il mouse sulle parole da correggere. \n \n Per annotare i dialoghi, clicca sulle parole nella nuvoletta azzurra e inserisci il nuovo testo (o nessun testo) con la casella in basso.";
    //            case Str.instructionsT:
    //                return "Istruzioni";
    //            case Str.important:
    //                return "IMPORTANTE:";
    //            case Str.importantInstructions:
    //                return "Se non sei sicura/o di come correggere una frase, puoi lasciarla com'? e premere INVIO oppure E. Se non sai come correggere una frase anche se ? strana o sbagliata, puoi semplicemente cancellare una parola, o se preferisci tutta la frase.";
    //            case Str.language:
    //                return "Lingua";
    //            case Str.save:
    //                return "Salva";
    //            case Str.load:
    //                return "Carica";
    //            case Str.exit:
    //                return "Esci";
    //            case Str.graphics:
    //                return "Grafica";
    //            case Str.pressStart:
    //                return "PREMI INVIO";
    //            case Str.answerThePhone:
    //                return "RISPONDERE";
    //            case Str.openDoor:
    //                return "APRIRE";
    //            case Str.pressFToRide:
    //                return "Premi F per salire!";
    //            case Str.WASDToMove:
    //                return "MUOVERTI";
    //            case Str.tutorialD0:
    //                return "Premi E";
    //            case Str.tutorialD1:
    //                return "Clicca una parola";
    //            case Str.tutorialD2:
    //                return "Inserisci il testo";
    //            case Str.tutorialD3:
    //                return "La frase cambia";
    //            case Str.tutorialG0:
    //                return "Premi E";
    //            case Str.tutorialG1:
    //                return "Clicca";
    //            case Str.tutorialG2:
    //                return "Trascina";
    //        }
    //    }
    //    return "";
    //}

    //public static string LocalizedMess(Player agent, Mess mess)
    //{
    //    Lang lang = Player.language;
    //    if (lang == Lang.eng)
    //    {
    //        switch (mess)
    //        {
    //            case Mess.fillOutAllItems:
    //                return "Please, fill out all of the items above!";
    //            case Mess.gameSaved:
    //                return "Saved! You can now close the game.";
    //            case Mess.goBackToGraffitiArea:
    //                return "Go back to the graffiti area!";
    //            case Mess.yourID:
    //                return "Your ID is " + agent.id + ". You can use it to restore your game save. Press ESC to show it.";
    //            case Mess.IDNotFound:
    //                return "ID not registered in our systems.";
    //            case Mess.pressToTalk:
    //                return $"Press {MultiplatformUtility.PrimaryInteractionKey} to talk!";
    //            case Mess.enterConversation:
    //                return $"Press {MultiplatformUtility.PrimaryInteractionKey} to enter the conversation!";
    //            case Mess.eraseOrReset:
    //                return $"Press {MultiplatformUtility.PrimaryInteractionKey} to erase! ({MultiplatformUtility.Cancel} to reset)";
    //            case Mess.closeConversation:
    //                return $"Press {MultiplatformUtility.PrimaryInteractionKey} to close the conversation!";
    //            case Mess.didntMeanToSayThis:
    //                return "Hey, I didn't mean to say this!";
    //            case Mess.notEnoughSoap:
    //                return "Not enough soap!";
    //            case Mess.insertYourNameFirst:
    //                return "Please insert your name first!";
    //            case Mess.iShouldAnswer:
    //                return "I should answer the phone.";
    //        }
    //    }
    //    else if (lang == Lang.ita)
    //    {
    //        switch (mess)
    //        {
    //            case Mess.fillOutAllItems:
    //                return "Per favore, completa tutte le voci!";
    //            case Mess.gameSaved:
    //                return "Salvato! Ora puoi chiudere il gioco.";
    //            case Mess.goBackToGraffitiArea:
    //                return "Torna alla zona dei graffiti!";
    //            case Mess.yourID:
    //                return "Il tuo ID ? " + agent.id + ". Puoi usarlo per recuperare la tua partita. Premi ESC per vederlo.";
    //            case Mess.IDNotFound:
    //                return "ID non registrato nei nostri sistemi.";
    //            case Mess.pressToTalk:
    //                return $"Premi {MultiplatformUtility.PrimaryInteractionKey} per parlare!";
    //            case Mess.enterConversation:
    //                return $"Premi {MultiplatformUtility.PrimaryInteractionKey} per ascoltare la conversazione!";
    //            case Mess.eraseOrReset:
    //                return $"Premi {MultiplatformUtility.PrimaryInteractionKey} per cancellare! ({MultiplatformUtility.Cancel} per ricominciare)";
    //            case Mess.closeConversation:
    //                return $"Premi {MultiplatformUtility.PrimaryInteractionKey} per chiudere la conversazione!";
    //            case Mess.didntMeanToSayThis:
    //                return Player.version == Version.Gram ? "Wow! Ormai parlo benissimo!" : "Ehi! Che mi succede? Ho sbagliato a parlare! Non volevo dire quello!";
    //            case Mess.notEnoughSoap:
    //                return "Non hai abbstanza sapone!";
    //            case Mess.insertYourNameFirst:
    //                return "Inserisci prima il tuo nome!";
    //            case Mess.iShouldAnswer:
    //                return "Dovrei rispondere al telefono.";
    //        }
    //    }
    //    return ""; 
    //}

    public static string GetLang(Lang lang)
    {
        if (lang == Lang.en)
            return "Eng";
        else if (lang == Lang.it)
            return "Ita";
        else
            return "";
    }
}

