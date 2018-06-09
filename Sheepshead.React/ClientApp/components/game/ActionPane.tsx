import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { IdUtils } from '../IdUtils';
import { FetchUtils } from '../FetchUtils';
import { render } from 'react-dom';
import DraggableCard from './DraggableCard';
import PickPane from './PickPane';

export interface ActionPaneState {
    gameId: string;
    playerId: string;
    playState: PlayState;
    pickChoices: any[];
}

export interface ActionPaneProps extends React.Props<any> {
    gameId: string;
}

export default class ActionPane extends React.Component<ActionPaneProps, ActionPaneState> {
    constructor(props: ActionPaneProps) {
        super(props);
        this.state = {
            gameId: props.gameId,
            playerId: IdUtils.getPlayerId(props.gameId) || '',
            playState: {
                turnType: '',
                humanTurn: false,
                requestingPlayerTurn: false,
                blinds: [],
                pickChoices: [],
                cardsPlayed: {},
                playerCards: []
            },
            pickChoices: []
        };
        this.initializePlayStatePinging = this.initializePlayStatePinging.bind(this);
        this.initializePlayStatePinging();
    }

    private initializePlayStatePinging(): void {
        var self = this;
        FetchUtils.repeatGet(
            'Game/GetPlayState?gameId=' + this.state.gameId + '&playerId=' + this.state.playerId,
            function (json: PlayState): void {
                self.setState({
                    playState: json,
                    pickChoices: json.pickChoices
                });
            },
            function (json: PlayState): boolean {
                return false; // json.requestingPlayerTurn == false;
            },
            1000);
    }

    private displayPhase(): string {
        if (this.state.playState == null)
            return 'Other';
        if (this.state.playState.turnType == 'Pick')
            return 'Pick';
        if (this.state.playState.turnType == 'Bury' && !this.state.playState.requestingPlayerTurn)
            return 'Pick';
        if (this.state.playState.turnType == 'Bury' && this.state.playState.requestingPlayerTurn)
            return 'Bury';
        else
            return 'Other';
    }

    private renderBury() {
        return (
            <div>
                <h4>Pick cards to bury</h4>
                {
                    this.state.playState.playerCards
                        ? this.state.playState.playerCards.map((card: string, i: number) =>
                            <DraggableCard key={i} cardImgNo={card} />
                        )
                        : (<div />)
                }
            </div>
        );
    }

    public render() {
        return (
            <div>
                {
                    this.displayPhase() == 'Pick'
                        ? <PickPane gameId={this.state.gameId}
                            pickChoices={this.state.pickChoices}
                            playerCards={this.state.playState.playerCards}
                            requestingPlayerTurn={this.state.playState.requestingPlayerTurn} />
                    : this.displayPhase() == 'Bury' ? this.renderBury()
                    : <h4>Other</h4>
                }
            </div>
        );
    }
}