class PlayState {
    turnType: string;
    humanTurn: boolean;
    requestingPlayerTurn: boolean;
    blinds: number[];
    pickChoices: any[];
    cardsPlayed: { [key: string]: string };
    playerCards: string[];
}