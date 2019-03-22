import React from 'react';
import { Carousel } from 'react-bootstrap';

export default function VerticalCarousel(props) {
    return (
        <div className="marquee-block">
            <div className="btn-pause play"></div>
            <Carousel className="vertical" onClick={props.onClick} indicators={false} controls={false} interval={5000}>
                {
                    props.messages.map((annoucement, index) =>
                        <Carousel.Item key={`annoucement${index}`}>
                            {annoucement}
                        </Carousel.Item>
                    )
                }
            </Carousel>
        </div>
 
    );
}