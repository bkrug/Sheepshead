import * as React from 'react';

export interface CardProps {
    cardImgNo: string,
    onClick?: (card: DraggableCard) => void
}

export default class DraggableCard extends React.Component<CardProps, any> {
    constructor(props: CardProps) {
        super(props);
        this.onClick = this.onClick.bind(this);
    }

    private onClick(e: any): void {
        if (this.props.onClick)
            this.props.onClick(this);
    }

    public render() {
        const { cardImgNo } = this.props;
        return (
            <img src={'./img/' + cardImgNo + '.png'} alt={cardImgNo} onClick={this.onClick} style={{ cursor: "pointer" }}/>
        )
	}
}