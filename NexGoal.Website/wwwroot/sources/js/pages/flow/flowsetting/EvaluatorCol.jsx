import React, { PureComponent, Fragment } from 'react';
import Select from 'react-select';

import { InputErrorCssClass } from '../../../constants';

export default class EvaluatorCol extends PureComponent {
    render() {
        const { isHost, isSubmit, evaluator, options } = this.props;

        return (
            <Fragment>
                {
                    isHost &&
                    <Select className={`react-select-container ${isSubmit && !evaluator && InputErrorCssClass}`}
                        classNamePrefix="react-select"
                        value={null} options={options}
                        isClearable={false} placeholder="Nickname or full name"
                        onChange={this.props.onChange}
                        backspaceRemovesValue={false}
                    />
                }
                {
                    evaluator &&
                    <div className="flow-item">
                        <img className="avatar" src={evaluator.avatar} />
                        <div className="personal-info">
                            <span className="txt-bold">{evaluator.nickName}</span>
                            <p>{evaluator.fullName}</p>
                        </div>
                    </div>
                }
            </Fragment>
        );
    }
}