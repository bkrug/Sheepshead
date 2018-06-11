export class PlayState {
    turnType: string;
    humanTurn: boolean;
    requestingPlayerTurn: boolean;
    blinds: number[];
    pickChoices: PickChoice[];
    cardsPlayed: TrickChoice[][];
    playerCards: string[];
    trickWinners: string[];
}

export class PickChoice {
    item1: string;
    item2: boolean;
}

export class TrickChoice {
    item1: string;
    item2: string;
}