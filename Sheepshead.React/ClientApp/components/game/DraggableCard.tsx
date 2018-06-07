import * as React from 'react';

export interface CardProps {
    cardImgNo: string
}

export default class DraggableCard extends React.Component<CardProps, any> {
    public render() {
        const { cardImgNo } = this.props;
        return (
            <img src={'./img/' + cardImgNo + '.png'} alt={cardImgNo} />
        )
	}
}