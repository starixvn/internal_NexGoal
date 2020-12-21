import React, { PureComponent } from 'react';

export default class Paragraph extends PureComponent {
    constructor(props) {
        super(props);

        this.state = {
            showCollapseExpandButton: false,
            isExpanded: false
        };
    }

    componentDidMount() {
        this.checkElementHeight();
    }

    toggleCollapseExpand = () => {
        this.setState(prevState => ({
            isExpanded: !prevState.isExpanded
        }));
    };

    checkElementHeight = () => {
        const height = this.refs.element.offsetHeight;
        if (height > 80) {
            this.setState({
                showCollapseExpandButton: true,
                isExpanded: true
            });
        }
    };

    render() {
        const seeMoreLessCssClass = this.state.isExpanded ? 'see-more' : 'see-less',
            seeMoreLessText = this.state.isExpanded ? '... see more' : '... see less';
        return (
            <div className="paragraph-wrapper" >
                <p className={this.state.isExpanded ? 'collapsed' : 'expanded'} ref="element">{this.props.content}</p>
                {
                    this.state.showCollapseExpandButton ?
                        <span className={seeMoreLessCssClass} onClick={this.toggleCollapseExpand}>
                            {seeMoreLessText}
                        </span>
                        : null
                }
            </div>
        );
    }
}