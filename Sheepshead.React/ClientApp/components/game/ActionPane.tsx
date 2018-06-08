import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { IdUtils } from '../IdUtils';
import { FetchUtils } from '../FetchUtils';
import { render } from 'react-dom';
import DraggableCard from './DraggableCard';

class PlayState {
    turnType: string;
    humanTurn: boolean;
    requestingPlayerTurn: boolean;
    blinds: number[];
    pickChoices: { [key: string]: boolean };
    cardsPlayed: { [key: string]: string };
    playerCards: string[];
}

export interface ActionPaneState {
    gameId: string;
    playerId: string;
    playState: PlayState;
    cardCount: number;
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
            playState: new PlayState,
            cardCount: 0
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
                    cardCount: json.playerCards.length
                });
            },
            function (json: PlayState): boolean {
                return true; // json.requestingPlayerTurn == false;
            },
            1000);
    }

    private pickChoice(willPick: boolean) : void {
        console.log(willPick);
        var self = this;
        FetchUtils.post(
            'Game/RecordPickChoice?gameId=' + this.state.gameId + '&playerId=' + this.state.playerId + '&willPick=' + willPick,
            function (json: number[]): void {
                self.initializePlayStatePinging();
            }
        );
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

    private renderChoices(given1: { [key: string]: boolean }): any[] {
        var retVal = [];
        for (var prop in given1) {
            retVal.push({
                playerName: prop,
                madeChoice: given1[prop]
            });
        }
        return retVal;
    }

    private renderPick() {
        return (
            <div>
                <h4>Pick Phase</h4>
                {
                    this.state.playState 
                    ?
                        this.renderChoices(this.state.playState.pickChoices).map(
                            (pickChoice: any, i: number) =>
                                <div key={i}>
                                    <p>{pickChoice.playerName + (pickChoice.madeChoice ? ' picked.' : ' refused.')}</p>
                                </div>
                        )
                    : <div></div>
                }
                <div>
                {
                    this.state.playState && this.state.playState.requestingPlayerTurn
                    ? <div>
                        <b>Do you want to pick?</b>
                        <button onClick={() => this.pickChoice(true)}>Yes</button>
                        <button onClick={() => this.pickChoice(false)}>No</button>
                        </div>
                    : <div></div>
                }
                </div>
            </div>
        );
    }

    private renderBury() {
        return (
            <div>
                <h4>Pick cards to bury</h4>
            </div>
        );
    }

    public render() {
        return (
            <div>
                {
                    this.displayPhase() == 'Pick' ? this.renderPick()
                    : this.displayPhase() == 'Bury' ? this.renderBury()
                    : <h4>Other</h4>
                }
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
}