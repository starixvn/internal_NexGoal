import React from 'react';
import { Carousel } from 'react-bootstrap';

export default function VerticalCarousel(props) {
    return (
        <Carousel className="warning vertical" onClick={props.onClick} indicators={false} controls={false} interval={5000}>
            <div className="btn-pause"></div>
            {
                props.messages.map((annoucement, index) =>
                    <Carousel.Item key={`annoucement${index}`}>
                         {annoucement}
                    </Carousel.Item>
                )
            }
        </Carousel>
    );
}