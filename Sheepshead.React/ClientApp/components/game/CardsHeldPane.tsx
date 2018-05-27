import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { IdUtils } from '../IdUtils';
import { FetchUtils } from '../FetchUtils';

interface Card {
    filename: string;
    cardAbbr: string;
}

export interface CardsHeldPaneState {
    gameId: string;
    playerId: string;
    cards: Card[]
}

export default class CardsHeldPane extends React.Component<any, any> {
    constructor(props: CardsHeldPaneState) {
        super(props);
        this.state = {
            gameId: props.gameId,
            playerId: IdUtils.getPlayerId(props.gameId),
            cards: []
        };
        var self = this;
        FetchUtils.get(
            'Game/GetCards?gameId=' + this.state.gameId + "&playerId=" + this.state.playerId,
            function (json: any): void {
                self.setState({ cards: json });
            }
        );
    }

    public render() {
        var cardList = this.state.cards.map((card: Card, i: number) =>
            <img key={i} src={'./img/' + card.filename + '.png'} alt={card.cardAbbr} />
        );
        return (
            <div>
                <h4>Cards Held Pane</h4>
                {cardList}
            </div>
        );
    }
}