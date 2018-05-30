import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { IdUtils } from '../IdUtils';
import { FetchUtils } from '../FetchUtils';

interface PlayState {
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
}

export default class ActionPane extends React.Component<any, any> {
    constructor(props: ActionPaneState) {
        super(props);
        this.state = {
            gameId: props.gameId,
            playerId: IdUtils.getPlayerId(props.gameId),
            playState: null
        };
        this.initializePlayStatePinging = this.initializePlayStatePinging.bind(this);
        this.initializePlayStatePinging();
    }

    private initializePlayStatePinging(): void {
        var self = this;
        FetchUtils.repeatGet(
            'Game/GetPlayState?gameId=' + this.state.gameId + '&playerId=' + this.state.playerId,
            function (json: PlayState): void {
                self.setState({ playState: json });
            },
            function (json: PlayState): boolean {
                return json.requestingPlayerTurn == false;
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

    private renderPick() {
        return (
            <div>
                <h4>Pick Phase</h4>
                {
                    this.state.playState.pickChoices.map(
                        (pickChoice: any, i: number) =>
                            <div key={i}>
                                <p>{pickChoice.item1 + (pickChoice.item2 ? ' picked.' : ' refused.')}</p>
                            </div>
                    )
                }
                <div>
                {
                    this.state.playState.requestingPlayerTurn
                        ? <div>
                            <b>Do you want to pick?</b>
                            <button onClick={() => this.pickChoice(true)}>Yes</button>
                            <button onClick={() => this.pickChoice(false)}>No</button>
                            </div>
                        : ''
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

    private renderCards() {
        return (
            <div>
                <h4>These are your cards</h4>
                {
                    this.state.playState == null
                        ? ''
                        : this.state.playState.playerCards.map((card: string, i: number) =>
                            <img key={i} src={'./img/' + card + '.png'} alt={card} />
                        )
                }
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
                    this.renderCards()
                }
            </div>
        );
    }
}