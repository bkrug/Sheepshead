export class PlayState {
    turnType: string;
    humanTurn: boolean;
    requestingPlayerTurn: boolean;
    blinds: number[];
    pickChoices: PickChoice[];
    cardsPlayed: { [key: string]: string };
    playerCards: string[];
}

export class PickChoice {
    item1: string;
    item2: boolean;
}