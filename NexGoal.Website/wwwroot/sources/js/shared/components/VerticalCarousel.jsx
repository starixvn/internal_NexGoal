import React, { PureComponent } from 'react';
import { Carousel } from 'react-bootstrap';

export default class VerticalCarousel extends PureComponent {
    constructor(props) {
        super(props);
        this.state = {
            slide: true,
            currentIndex: 0
        };
    }

    toggleSlide = () => {
        const currentIndex = this.refs.carousel.state.activeIndex;
        this.setState(prevState => ({ slide: !prevState.slide, currentIndex: currentIndex }));
    };

    renderMessage = (message, index) =>
        <Carousel.Item key={`message${index}`} active={true} className="carousel-item" data-index={index}>
            {message}
        </Carousel.Item>;

    render() {
        return (
            <div className="marquee-block">
                <div className={`btn-pause ${!this.state.slide && 'play'}`} onClick={this.toggleSlide} />
                <Carousel ref='carousel' key={this.state.slide} className="vertical" slide={this.state.slide} onClick={this.props.onClick}
                    indicators={false} controls={false} interval={5000}
                    defaultActiveIndex={this.state.currentIndex}>
                    {this.props.messages.map(this.renderMessage)}
                </Carousel>
            </div>

        );
    }
}