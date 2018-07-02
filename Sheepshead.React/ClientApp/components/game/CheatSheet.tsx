import '../../css/game.css';
import * as React from 'react';
import { RouteComponentProps } from 'react-router';

export interface CheatState {
    display: boolean
}

export interface CheatProps extends React.Props<any> {
}

export class CheatSheet extends React.Component<CheatProps, CheatState> {
    constructor(props: CheatProps) {
        super(props);
        this.state = {
            display: false
        };
        this.showModal = this.showModal.bind(this);
        this.hideModal = this.hideModal.bind(this);
    }

    private showModal(): void {
        this.setState({
            display: true
        });
    }

    private hideModal(): void {
        this.setState({
            display: false
        });
    }

    private renderModal() {
        return (
            <div className="modalDialog">
                <div>
                    Cheat Sheet
                </div>
            </div>
        );
    }

    public render() {
        return (
            <div>
                <a style={{ cursor: 'pointer' }} onMouseOver={this.showModal} onMouseOut={this.hideModal}>Show Cheat Sheet</a>
                { this.state.display ? this.renderModal() : <div></div> }
            </div>
        );
    }
}