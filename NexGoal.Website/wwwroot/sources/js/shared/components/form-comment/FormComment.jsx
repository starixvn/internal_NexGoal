import React, { Component } from 'react';
import { FormGroup } from 'react-bootstrap';
import getResource from '../../../resourceHelper';
import PropTypes from 'prop-types';

export default class FormComment extends Component {

    static propTypes = {
        kpiEoyQuestions: PropTypes.array,
        scores: PropTypes.array,
        handleChange: PropTypes.func,
        values: PropTypes.object,
        touched: PropTypes.object,
        errors: PropTypes.object,
        inputErrorCssClass: PropTypes.string,
        setFieldValue: PropTypes.func,
        isAction: PropTypes.bool,
        isDisabled: PropTypes.bool
    };

    static defaultProps = {
        kpiEoyQuestions: [],
        scores: [],
        values: { rating: [] },
        touched: {},
        errors: {},
        inputErrorCssClass: 'none',
        isAction: true,
        isDisabled: false
    };

    constructor (props) {
        super(props);
        this.state = {
            scores: props.scores,
            rates: props.scores
        };
    }

    componentDidUpdate(prevProps) {
        if (JSON.stringify(prevProps.scores) !== JSON.stringify(this.props.scores)) {
            this.setState({
                scores: this.props.scores,
                rates: this.props.scores
            });
        }
    }

    handleMouseOver = (star, index) => {
        const scores = [...this.state.scores];
        scores[index] = star;

        this.setState({ scores: scores });
    };

    handleMouseLeave = () => {
        this.setState({ scores: this.state.rates });
    };

    handleClick = (star, index) => {
        const rates = [...this.state.rates];
        rates[index] = star;
        this.setState({ rates: rates, scores: rates });

        if (this.props.values.rating) {
            const rating = [...this.props.values.rating];
            rating[index].rate = star;
            this.props.setFieldValue('rating', rating);
        }
    };

    handleClearStar = index => {
        const scores = [...this.state.scores];
        scores[index] = 0;
        const rating = [...this.props.values.rating];
        rating[index].rate = 0;

        this.setState({ scores: scores, rates: scores });
        this.props.setFieldValue('rating', rating);
    };

    renderStar = index => {
        const stars = [1, 2, 3, 4, 5];
        const scores = this.state.scores;
        const isAction = this.props.isAction;

        return stars.map((item, key) => (
            <span className="star" key={key}
                onMouseOver={() => isAction && this.handleMouseOver(item, index)}
                onMouseLeave={() => isAction && this.handleMouseLeave()}
                onClick={() => isAction && this.handleClick(item, index)}
            >
                <span className="layer-one"><i className="fa fa-star fa-lg" /></span>
                <span className="layer-two" style={ scores[index] >= item ? null : {visibility: 'hidden', display: 'none'}}><i className="fa fa-star fa-lg" /></span>
            </span>
        ));
    };

    render() {
        const { kpiEoyQuestions, values, touched, errors, inputErrorCssClass, handleChange, isDisabled, isAction } = this.props;
        if (!kpiEoyQuestions) {
            return null;
        }

        return (
            <div className="form-comment">
                <span>{getResource('DirectionProvide')}</span>
                {
                    kpiEoyQuestions.map((item, key) => (
                        <FormGroup key={key}>
                            <span className="criteria">{item.name}</span>
                            {
                                item.kpis.length > 0 && item.kpis.map((element, index) => (
                                    <div className="kpi-group" key={index}>
                                        <span className="kpi-comment">{element.kpiName}</span>
                                        {
                                            isDisabled ?
                                            (
                                                <span
                                                    className="span-area"
                                                    role="textbox"
                                                    contenteditable="false"
                                                >
                                                    { values[`${element.kpiId}`] }
                                                </span>
                                            ) : (
                                                <textarea
                                                    className={`text-area ${touched[`${element.kpiId}`] && errors[`${element.kpiId}`] && inputErrorCssClass}`}
                                                    rows="2"
                                                    disabled={isDisabled}
                                                    value={values[`${element.kpiId}`]}
                                                    name={`${element.kpiId}`}
                                                    onChange={handleChange}
                                                />
                                            )
                                        }
                                    </div>
                                ))
                            }
                        </FormGroup>
                    ))
                }
                <FormGroup>
                    <div className="rating">
                        <span className="criteria">{getResource('RatingCriteria')}</span>
                        {
                            kpiEoyQuestions.map((item, index) => (
                                <div className="full-star" key={index}>
                                    <span className="criteria">{item.name}</span>
                                    { isAction && <span className="clear" onClick={() => this.handleClearStar(index)}><i className="fa fa-times-circle-o" /></span> }
                                    { this.renderStar(index) }
                                </div>
                            ))
                        }
                    </div>
                </FormGroup>
            </div>
        );
    }
}