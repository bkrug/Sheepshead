class PlayState {
    turnType: string;
    humanTurn: boolean;
    requestingPlayerTurn: boolean;
    blinds: number[];
    pickChoices: { [key: string]: boolean };
    cardsPlayed: { [key: string]: string };
    playerCards: string[];
}