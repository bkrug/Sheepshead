export class PlayState {
    turnType: string;
    currentTurn: string;
    requestingPlayerTurn: boolean;
    blinds: CardSummary[];
    pickChoices: PickChoice[];
    cardsPlayed: TrickChoice[][];
    playerCards: CardSummary[];
    trickWinners: string[];
}

export class PickState {
    turnType: string;
    currentTurn: string;
    requestingPlayerTurn: boolean;
    pickChoices: PickChoice[];
    playerCards: CardSummary[];
    mustRedeal: boolean;
}

export class BuryState {
    turnType: string;
    requestingPlayerTurn: boolean;
    blinds: CardSummary[];
    playerCards: CardSummary[];
    legalCalledAces: CardSummary[];
    partnerMethod: string;
}

export class HandSummary {
    points: any;
    coins: any;
    mustRedeal: boolean;
    tricks: { key: string, value: CardSummary[] }[];
}

export class PickChoice {
    item1: string;
    item2: boolean;
}

export class TrickChoice {
    item1: string;
    item2: CardSummary;
}

export class CardSummary {
    name: string;
    filename: string;
    legalMove: boolean | null;
    abbreviation: string;
}

export interface GameScore {
    name: string;
    score: number;
}

export class HandScores {
    coins: GameScore[];
    points: GameScore[];
}
